/*
Matrix.cs, classe qui permet d'effectuer des opérations mathématiques sur les matrices
Trystan Piette
Décembre 2023
*/

using System;
using System.Text;

namespace PIF1006_tp2 {
  public class Matrix2D {
    public double[, ] Matrix {
      get;
      private set;
    }
    public string Name {
      get;
      private set;
    }

    public Matrix2D(string name, int lines, int columns) {
      Matrix = new double[lines, columns];
      Name = name;
    }

    public Matrix2D Transpose() {
      int rows = Matrix.GetLength(0);
      int columns = Matrix.GetLength(1);

      Matrix2D transposedMatrix = new(Name + "_Transposée", columns, rows);

      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < columns; j++) {
          transposedMatrix.Matrix[j, i] = Matrix[i, j];
        }
      }

      return transposedMatrix;
    }

    public bool IsSquare() {
      int rows = Matrix.GetLength(0);
      int columns = Matrix.GetLength(1);

      return rows == columns;
    }

    public double Determinant() {
      if (!IsSquare()) {
        throw new InvalidOperationException("Determinant est seulement défini pour les matrices carrées.");
      }

      if (Matrix.GetLength(0) == 2 && Matrix.GetLength(1) == 2) {
        return Matrix[0, 0] * Matrix[1, 1] - Matrix[0, 1] * Matrix[1, 0];
      }

      double determinant = 0;

      for (int i = 0; i < Matrix.GetLength(0); i++) {
        determinant += Matrix[0, i] * Cofactor(0, i);
      }

      return determinant;
    }

    private double Cofactor(int row, int col) {
      Matrix2D minorMatrix = GetMinor(row, col);
      double minorDeterminant = minorMatrix.Determinant();

      return (row + col) % 2 == 0 ? minorDeterminant : -minorDeterminant;
    }

    private Matrix2D GetMinor(int row, int col) {
      int rows = Matrix.GetLength(0);
      int columns = Matrix.GetLength(1);

      Matrix2D minorMatrix = new("Minor", rows - 1, columns - 1);

      for (int i = 0, newRow = 0; i < rows; i++) {
        if (i == row) {
          continue;
        }

        for (int j = 0, newCol = 0; j < columns; j++) {
          if (j == col) {
            continue;
          }

          minorMatrix.Matrix[newRow, newCol] = Matrix[i, j];
          newCol++;
        }

        newRow++;
      }

      return minorMatrix;
    }

    public Matrix2D Comatrix() {
      if (!IsSquare()) {
        throw new InvalidOperationException("Comatrix est seulement définie pour les matrices carrées.");
      }

      int rows = Matrix.GetLength(0);
      int columns = Matrix.GetLength(1);

      Matrix2D comatrix = new(Name + "_Comatrix", rows, columns);

      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < columns; j++) {
          comatrix.Matrix[i, j] = Cofactor(i, j);
        }
      }

      return comatrix;
    }

    public Matrix2D Inverse() {
      double determinant = Determinant();

      if (determinant == 0 || !IsSquare()) {
        return null;
      }

      int rows = Matrix.GetLength(0);
      int columns = Matrix.GetLength(1);

      Matrix2D inverseMatrix = new(Name + "_Inversée", rows, columns);

      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < columns; j++) {
          inverseMatrix.Matrix[i, j] = Cofactor(j, i) / determinant;
        }
      }

      return inverseMatrix;
    } 

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"{Name}:");

      for (int i = 0; i < Matrix.GetLength(0); i++) {
        sb.Append("| ");

        for (int j = 0; j < Matrix.GetLength(1); j++) {
          sb.Append($"{Matrix[i, j]} ");
        }

        sb.AppendLine("|");
      }

      return sb.ToString();
    }

    public Matrix2D Augment(Matrix2D matrix) {
      if (matrix.Matrix.GetLength(0) != Matrix.GetLength(0)) {
        throw new InvalidOperationException("Ls matrices doivent contenir les même nombre de lignes.");
      }

      int rows = Matrix.GetLength(0);
      int colsA = Matrix.GetLength(1);
      int colsB = matrix.Matrix.GetLength(1);

      Matrix2D result = new(Name, rows, colsA + colsB);

      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < colsA; j++) {
          result.Matrix[i, j] = Matrix[i, j];
        }

        for (int j = 0; j < colsB; j++) {
          result.Matrix[i, colsA + j] = matrix.Matrix[i, j];
        }
      }

      return result;
    }
    public static double[, ] MultiplyMatrix(double[, ] A, double[, ] B) {
      int rowsA = A.GetLength(0);
      int colsA = A.GetLength(1);
      int rowsB = B.GetLength(0);
      int colsB = B.GetLength(1);

      if (colsA != rowsB) {
        throw new InvalidOperationException("Matrices ne peuvent pas être multipliés car les formats sont invalides.");
      }

      double[, ] resultMatrix = new double[rowsA, colsB];

      for (int i = 0; i < rowsA; i++) {
        for (int j = 0; j < colsB; j++) {
          double sum = 0.0;
          for (int k = 0; k < colsA; k++) {
            sum += A[i, k] * B[k, j];
          }
          resultMatrix[i, j] = sum;
        }
      }

      return resultMatrix;
    }
  }
}