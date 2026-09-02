using UnityEngine;

public static class UIGradientUtils
{
	public struct Matrix2x3
	{
		public float m00, m01, m02, m10, m11, m12;
		public Matrix2x3(float m00, float m01, float m02, float m10, float m11, float m12)
		{
			this.m00 = m00;
			this.m01 = m01;
			this.m02 = m02;
			this.m10 = m10;
			this.m11 = m11;
			this.m12 = m12;
		}

		public static Vector2 operator*(Matrix2x3 m, Vector2 v)
		{
			float x = (m.m00 * v.x) - (m.m01 * v.y) + m.m02;
			float y = (m.m10 * v.x) + (m.m11 * v.y) + m.m12;
			return new Vector2(x, y);
		}
	}

	public static Matrix2x3 LocalPositionMatrix(Rect rect, Vector2 dir)
	{
		float cos = dir.x;
		float sin = dir.y;
		Vector2 rectMin = rect.min;
		Vector2 rectSize = rect.size;
		float c = 0.5f;
		float ax = rectMin.x / rectSize.x + c;
		float ay = rectMin.y / rectSize.y + c;
		float m00 = cos / rectSize.x;
		float m01 = sin / rectSize.y;
		float m02 = -(ax * cos - ay * sin - c);
		float m10 = sin / rectSize.x;
		float m11 = cos / rectSize.y;		
		float m12 = -(ax * sin + ay * cos - c);
		return new Matrix2x3(m00, m01, m02, m10, m11, m12);
	}

	public static Vector2 RotationDir(float angle)
	{
		float angleRad = angle * Mathf.Deg2Rad;
		float cos = Mathf.Cos(angleRad);
		float sin = Mathf.Sin(angleRad);
		return new Vector2(cos, sin);
	}

	public static Vector2 CompensateAspectRatio(Rect rect, Vector2 dir)
	{
		float ratio = rect.height / rect.width;
		dir.x *= ratio;
		return dir.normalized;
	}
}
