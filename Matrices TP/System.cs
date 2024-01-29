/*
System.cs, classe qui permet d'effectuer les méthodes mathématiques sur les matrices
Trystan Piette
Décembre 2023
*/
using System;
using System.Text;

namespace PIF1006_tp2 {
  public class System {
    public Matrix2D A {
      get;
      private set;
    }
    public Matrix2D B {
      get;
      private set;
    }

    public System(Matrix2D a, Matrix2D b) {
      A = a;
      B = b;
    }

    public bool IsValid() {

      int rowsA = A.Matrix.GetLength(0);
      int rowsB = B.Matrix.GetLength(0);
      int columnsB = B.Matrix.GetLength(1);

      if (A.IsSquare() && rowsA == rowsB && columnsB == 1) {
        return true;
      } else
        return false;
    }

    public double[] SolveUsingCramer() {
      if (!IsValid()) {
        throw new InvalidOperationException("La matrice ne concorde pas avec le système pour la règle de Cramer.");
      }

      double determinantA = A.Determinant();

      if (Math.Abs(determinantA) < double.Epsilon) {
        throw new InvalidOperationException("La règle de Cramer ne peut pas être utilisée car le déterminant de la matrice est égal à zéro. (0 ou ∞ solutions)");
      }

      int n = A.Matrix.GetLength(0); // Assuming A is an n x n matrix
      double[] solutions = new double[n];

      for (int i = 0; i < n; i++) {
        Matrix2D modifiedMatrix = CreateModifiedMatrix(i, B); // Récursivité qui permet de créer la nouvelle matrice résultante
        solutions[i] = modifiedMatrix.Determinant() / A.Determinant();
      }

      return solutions;
    }

    private Matrix2D CreateModifiedMatrix(int column, Matrix2D constants) {
      int rows = A.Matrix.GetLength(0);
      int columns = A.Matrix.GetLength(1);

      Matrix2D modifiedMatrix = new("ModifiedMatrix", rows, columns);

      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < columns; j++) {
          if (j == column) {
            modifiedMatrix.Matrix[i, j] = constants.Matrix[i, 0];
          } else {
            modifiedMatrix.Matrix[i, j] = A.Matrix[i, j];
          }
        }
      }

      return modifiedMatrix;
    }

    public double[, ] SolveUsingInverseMatrix() {
      if (!IsValid()) {
        throw new InvalidOperationException("La matrice ne concorde pas avec le système pour la méthode de la matrice inverse.");
      }

      double determinantA = A.Determinant();

      if (Math.Abs(determinantA) < double.Epsilon) {
        throw new InvalidOperationException("La règle de Cramer ne peut pas être utilisée car le déterminant de la matrice est égal à zéro. (0 ou ∞ solutions)");
      }

      Matrix2D inverseA = A.Inverse();

      double[, ] inverseAMatrix = inverseA.Matrix;
      double[, ] BMatrix = B.Matrix;

      double[, ] resultArray = Matrix2D.MultiplyMatrix(inverseAMatrix, BMatrix);


      return resultArray;
    }

    public double[] SolveUsingGauss() {
      if (!IsValid()) {
        throw new InvalidOperationException("La matrice ne concorde pas avec le système pour la méthode de Gauss.");
      }

      int n = A.Matrix.GetLength(0);

      double[, ] augmentedMatrix = new double[n, n + 1];
      for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
          augmentedMatrix[i, j] = A.Matrix[i, j];
        }
        augmentedMatrix[i, n] = B.Matrix[i, 0];
      }

      // Élimination gaussienne
      for (int i = 0; i < n; i++) {
        // Vérifie si le pivot est 0
        if (augmentedMatrix[i, i] == 0) {
          throw new InvalidOperationException("Pivot zéro, il y a aucune solution ou infini.");
        }

        // Crée l'élément diagonal 1
        double diagonalElement = augmentedMatrix[i, i];
        for (int j = 0; j <= n; j++) {
          augmentedMatrix[i, j] /= diagonalElement;
        }

        for (int k = 0; k < n; k++) {
          if (k != i) {
            double factor = augmentedMatrix[k, i];
            for (int j = 0; j <= n; j++) {
              augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
            }
          }
        }
      }

      double[] solutions = new double[n];
      for (int i = 0; i < n; i++) {
        solutions[i] = augmentedMatrix[i, n];
      }

      return solutions;
    }

    public double[] SolveUsingJacobi(int maxIterations = 100, double tolerance = 1e-6) {
      if (!IsValid()) {
        throw new InvalidOperationException("La matrice ne concorde pas avec le système pour la méthode de Jacobi.");
      }

      int n = A.Matrix.GetLength(0);
      double[] x = new double[n];
      double[] xNext = new double[n];

      for (int iteration = 0; iteration < maxIterations; iteration++) {
        for (int i = 0; i < n; i++) {
          double sum = B.Matrix[i, 0];
          for (int j = 0; j < n; j++) {
            if (j != i) {
              sum -= A.Matrix[i, j] * x[j];
            }
          }
          xNext[i] = sum / A.Matrix[i, i];
        }

        // Regarde pour convergence
        bool converged = true;
        for (int i = 0; i < n; i++) {
          if (Math.Abs(xNext[i] - x[i]) > tolerance) {
            converged = false;
            break;
          }
        }

        Console.WriteLine($"Itération {iteration + 1}: {string.Join(", ", xNext)}");

        if (converged) {
          return xNext;
        }

        Array.Copy(xNext, x, n);
      }

      throw new InvalidOperationException("Jacobi n'a pas converti selon le nombre d'itérations max.");
    }

    public double[] SolveUsingGaussSeidel(int maxIterations = 100, double tolerance = 1e-10) {
      if (!IsValid()) {
        throw new InvalidOperationException("La matrice ne concorde pas avec le système pour la méthode de Gauss-Seidel.");
      }

      int n = A.Matrix.GetLength(0);
      double[] x = new double[n];

      for (int iteration = 0; iteration < maxIterations; iteration++) {
        for (int i = 0; i < n; i++) {
          double sum = B.Matrix[i, 0];
          for (int j = 0; j < n; j++) {
            if (j != i) {
              sum -= A.Matrix[i, j] * x[j];
            }
          }
          x[i] = sum / A.Matrix[i, i];
        }

        // Regarde pour convergence
        double residual = CalculateResidual(A, x, B);
        Console.WriteLine($"Itération {iteration + 1}: {string.Join(", ", x)}");

        if (residual < tolerance) {
          return x;
        }
      }

      throw new InvalidOperationException("Gauss-Seidel method did not converge within the specified number of iterations.");
    }

    static double CalculateResidual(Matrix2D matrice, double[] x, Matrix2D constantes) {
      int n = matrice.Matrix.GetLength(0);
      double[] Ax = new double[n];

      // Calcule le produit de la matrice des coefficients (matrice) et du vecteur solution (x)
      for (int i = 0; i < n; i++) {
        double somme = 0;
        for (int j = 0; j < n; j++) {
          somme += matrice.Matrix[i, j] * x[j];
        }
        Ax[i] = somme;
      }

      // Calcule le vecteur résiduel en soustrayant le produit du vecteur des constantes
      double[] vecteurResiduel = new double[n];
      for (int i = 0; i < n; i++) {
        vecteurResiduel[i] = constantes.Matrix[i, 0] - Ax[i];
      }

      // Calcule la norme euclidienne (magnitude) du vecteur résiduel
      double residu = 0;
      for (int i = 0; i < n; i++) {
        residu += Math.Pow(vecteurResiduel[i], 2);
      }

      return Math.Sqrt(residu);
    }

    public override string ToString() {
      StringBuilder sb = new();

      int rowsA = A.Matrix.GetLength(0);
      int columnsA = A.Matrix.GetLength(1);

      for (int i = 0; i < rowsA; i++) {
        for (int j = 0; j < columnsA; j++) {
          sb.AppendFormat("{0}x{1}", A.Matrix[i, j], j + 1);

          if (j < columnsA - 1) {
            sb.Append(" + ");
          }
        }

        sb.AppendFormat(" = {0}", B.Matrix[i, 0]);

        if (i < rowsA - 1) {
          sb.AppendLine();
        }
      }

      return sb.ToString();
    }
  }
}