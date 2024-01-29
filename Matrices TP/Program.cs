/*
Program.cs, menu qui permet d'effectuer les opérations des classes System.cs et matrix.cs
Trystan Piette
Décembre 2023
*/

using System;
using System.IO;

namespace PIF1006_tp2 {

  class Program {

    static void Main(string[] args) {
      System system = null;

      while (true) { // Menu des matrices et leurs opérations
        Console.WriteLine("------ Menu de matrices ------");
        Console.WriteLine("1. Charger un fichier de système");
        Console.WriteLine("2. Afficher le système");
        Console.WriteLine("3. Résoudre avec Cramer");
        Console.WriteLine("4. Résoudre avec la méthode de la matrice inverse");
        Console.WriteLine("5. Résoudre avec Gauss");
        Console.WriteLine("6. Résoudre avec Gauss-Seidel et Jacobi selon votre epsilon");
        Console.WriteLine("7. Quitter");
        Console.Write("Choix: ");
        Console.Write(" ");

        string choix = Console.ReadLine();

        switch (choix) {
        case "1": // permet de loder une matrice
          system = LoadSystemFromFile();
          break;

        case "2":
          if (system != null) { // permet l'affichage des matrices
            Console.WriteLine("------ Affichage du système ------");
            Console.WriteLine(system);
          } else {
            Console.WriteLine("Veuillez d'abord charger un fichier de système.");
          }
          break;

        case "3": // permet de résoudre à l'aide de Cramer
          if (system != null) {
            try {
              double[] resultArray = system.SolveUsingCramer();
              Matrix2D matrixX = ConvertToMatrix(resultArray, "Résultat");
              Console.WriteLine("Solution avec Cramer:\n" + matrixX);
            } catch (InvalidOperationException ex) {
              Console.WriteLine(ex.Message);
            }
          } else {
            Console.WriteLine("Veuillez d'abord charger un fichier de système.");
          }
          break;

        case "4": // permet de résoudre à l'aide de la matrice inverse
          if (system != null) {
            try {
              double[, ] resultArray = system.SolveUsingInverseMatrix();
              Matrix2D matrixX = new("Résultat", resultArray.GetLength(0), resultArray.GetLength(1));

              for (int i = 0; i < resultArray.GetLength(0); i++) {
                for (int j = 0; j < resultArray.GetLength(1); j++) {
                  matrixX.Matrix[i, j] = resultArray[i, j];
                }
              }

              Console.WriteLine("Solution avec la méthode de la matrice inverse:\n" + matrixX);
            } catch (InvalidOperationException ex) {
              Console.WriteLine(ex.Message);
            }
          } else {
            Console.WriteLine("Veuillez d'abord charger un fichier de système.");
          }
          break;

        case "5": // permet de résoudre à l'aide de Gauss
          if (system != null) {
            try {
              double[] resultArray = system.SolveUsingGauss();
              Matrix2D matrixX = ConvertToMatrix(resultArray, "Résultat");
              Console.WriteLine("Solution avec Gauss:\n" + matrixX);
            } catch (InvalidOperationException ex) {
              Console.WriteLine(ex.Message);
            }
          } else {
            Console.WriteLine("Veuillez d'abord charger un fichier de système.");
          }
          break;

        case "6":
          // Résoudre avec Gauss-Seidel et Jacobi
          if (system != null) {
            Console.Write("Entrez la valeur d'epsilon : ");
            double epsilon = double.Parse(Console.ReadLine());

            try {
              Console.WriteLine("------ Résolution avec Gauss-Seidel ------");
              double[] resultGaussSeidel = system.SolveUsingGaussSeidel(tolerance: epsilon);
              DisplaySolution(resultGaussSeidel);

              Console.WriteLine("------ Résolution avec Jacobi ------");
              double[] resultJacobi = system.SolveUsingJacobi(tolerance: epsilon);
              DisplaySolution(resultJacobi);
            } catch (InvalidOperationException ex) {
              Console.WriteLine(ex.Message);
            }
          } else {
            Console.WriteLine("Veuillez d'abord charger un fichier de système.");
          }
          break;

        case "7": // quitter le programme
          Environment.Exit(0);
          break;

        default: // si choix invalide
          Console.WriteLine("Choix invalide. Veuillez entrer un numéro entre 1 et 6.");
          break;
        }

        Console.WriteLine(); // Ajoute une ligne pour la visibilité du texte
      }
    }

    static Matrix2D ConvertToMatrix(double[] array, string name) { // permet de convertir le fichier txt. en format adapatable pour les manipulations de matrix2D
      Matrix2D matrixX = new(name, array.Length, 1);

      for (int i = 0; i < array.Length; i++) {
        matrixX.Matrix[i, 0] = array[i];
      }

      return matrixX;
    }

    // Fonction qui permet de lire un fichier txt. et de le transformer en deux matrices A et B selon le format suivant:
    /*
    3 3  (le format de la matrice A, doit être carré pour le bon fonctionnement)
    x y z  (Ligne 1 de la matrice A, où que chaque position est ratachée à une variable x,y ou z)
    x y z
    x y z

    a  (matrice B à laquelle on doit résoudre)
    b
    c
    */
    static System LoadSystemFromFile() {
      Console.Write("Enter le nom du fichier: ");
      string filePath = Console.ReadLine();

      try {
        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length < 3) {
          Console.WriteLine("Format invalid de fichier. S'assurer qu'il y a deux matrices.");
          return null;
        }

        string[] sizeTokens = lines[0].Split(' ');
        if (sizeTokens.Length != 2 || !int.TryParse(sizeTokens[0], out int numRows) || !int.TryParse(sizeTokens[1], out int numCols)) {
          Console.WriteLine("Format invalid de fichier. La première ligne détermine le format de la matrice A.");
          return null;
        }

        Matrix2D matrixA = new("A", numRows, numCols);
        Matrix2D matrixB = new("B", numRows, 1);

        int lineIndex = 1;

        for (int i = 0; i < numRows; i++) {
          string[] rowTokens = lines[lineIndex].Split(' ');

          if (rowTokens.Length != numCols) {
            Console.WriteLine($"Format invalid de fichier. Ligne {lineIndex + 1} ne correspond pas au bon nombre d'éléments pour la matrice A.");
            return null;
          }

          for (int j = 0; j < numCols; j++) {
            if (!double.TryParse(rowTokens[j], out double value)) {
              Console.WriteLine($"Format invalid de fichier. Élément non-numérique à la ligne {lineIndex + 1}, column {j + 1} de la matrice A.");
              return null;
            }

            matrixA.Matrix[i, j] = value;
          }

          lineIndex++;
        }

        lineIndex++;

        for (int i = 0; i < numRows; i++) {
          if (!double.TryParse(lines[lineIndex], out double value)) {
            Console.WriteLine($"Format invalid de fichier. Élément non-numérique à la ligne {lineIndex + 1} de la matrice B.");
            return null;
          }

          matrixB.Matrix[i, 0] = value;
          lineIndex++;
        }

        Console.WriteLine("Le fichier a été téléchargé avec succès.");
        return new System(matrixA, matrixB);
      } catch (Exception ex) {
        Console.WriteLine($"Erreur de lecture du fichier: {ex.Message}");
        return null;
      }
    }

    static void DisplaySolution(double[] result) {
      Console.WriteLine("Résultat:");
      for (int i = 0; i < result.Length; i++) {
        Console.WriteLine($"| {result[i]} |");
      }
      Console.WriteLine();
    }

  }
}