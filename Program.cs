using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpSwitchRegression {
	class Program {

		private const int NUM_ARMS = 10_000;

		static void Main( string[] args ) {
			var compilation = CSharpCompilation.Create(
				assemblyName: "SomeAssm",
				syntaxTrees: new[] { MakeTree() },
				options: new CSharpCompilationOptions(
					outputKind: OutputKind.DynamicallyLinkedLibrary,
					optimizationLevel: OptimizationLevel.Release,
					deterministic: true
				)
			);

			Console.WriteLine( "Starting compilation" );
			Stopwatch sw = new();
			sw.Start();
			compilation.Emit( outputPath: "SomeAssm.dll" );
			sw.Stop();

			Console.WriteLine( $"Completed compilation in { sw.ElapsedMilliseconds }ms" );
		}

		private static SyntaxTree MakeTree() {
			var method = SyntaxFactory.MethodDeclaration(
				returnType: SyntaxFactory.PredefinedType( SyntaxFactory.Token( SyntaxKind.ObjectKeyword ) ),
				identifier: "SomeMethod"
			).WithParameterList( SyntaxFactory.ParameterList( SyntaxFactory.SingletonSeparatedList(
				SyntaxFactory
					.Parameter( SyntaxFactory.Identifier( "str" ) )
					.WithType( SyntaxFactory.PredefinedType( SyntaxFactory.Token( SyntaxKind.StringKeyword ) ) )
			) ) )
			.WithExpressionBody( SyntaxFactory.ArrowExpressionClause( SyntaxFactory.SwitchExpression(
				governingExpression: SyntaxFactory.IdentifierName( SyntaxFactory.Identifier( "str" ) ),
				arms: SyntaxFactory.SeparatedList( MakeArms() )
			) ) );

			var clazz = SyntaxFactory
				.ClassDeclaration( "SomeClass" )
				.WithModifiers( SyntaxFactory.TokenList(
				   SyntaxFactory.Token( SyntaxKind.PublicKeyword ),
				   SyntaxFactory.Token( SyntaxKind.SealedKeyword )
				) )
				.WithMembers( SyntaxFactory.SingletonList<MemberDeclarationSyntax>( method ) );


			var ns = SyntaxFactory
				.NamespaceDeclaration(
					SyntaxFactory.IdentifierName( "SomeNs" )
				)
				.WithMembers( SyntaxFactory.SingletonList<MemberDeclarationSyntax>( clazz ) );

			var tree = SyntaxFactory.SyntaxTree(
				SyntaxFactory
					.CompilationUnit()
					.WithMembers( SyntaxFactory.SingletonList<MemberDeclarationSyntax>( ns ) )
			);

			return tree;
		}

		private static string MakeCase() {
			return Guid.NewGuid().ToString() + typeof( Program ).AssemblyQualifiedName;
		}

		private static IEnumerable<SwitchExpressionArmSyntax> MakeArms() {
			for( int i = 0; i < NUM_ARMS; ++i ) {
				yield return SyntaxFactory.SwitchExpressionArm(
					SyntaxFactory.ConstantPattern(
						SyntaxFactory.LiteralExpression(
							SyntaxKind.StringLiteralExpression,
							SyntaxFactory.Literal( MakeCase() )
						)
					),
					SyntaxFactory.LiteralExpression(
						SyntaxKind.NullLiteralExpression
					)
				);
			}

			yield return SyntaxFactory.SwitchExpressionArm(
				SyntaxFactory.DiscardPattern(),
				SyntaxFactory.LiteralExpression(
					SyntaxKind.NullLiteralExpression
				)
			);
		}
	}
}
