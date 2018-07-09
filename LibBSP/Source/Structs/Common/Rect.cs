#if !(UNITY || UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER)
using System;
using System.Collections.Generic;

namespace LibBSP {
	/// <summary>
	/// Struct defining a rectangle in 2D space.
	/// </summary>
	public struct Rect : IEquatable<Rect> {

		public Vector2d position;
		public Vector2d size;

		/// <summary>
		/// Gets the center point of this <see cref="Rect"/>.
		/// </summary>
		public Vector2d center {
			get {
				return position + (size / 2.0);
			}
		}

		/// <summary>
		/// Gets the width of this <see cref="Rect"/>.
		/// </summary>
		public double width {
			get {
				return size.x;
			}
			set {
				size.x = value;
			}
		}

		/// <summary>
		/// Gets the height of this <see cref="Rect"/>.
		/// </summary>
		public double height {
			get {
				return size.y;
			}
			set {
				size.y = value;
			}
		}

		/// <summary>
		/// Gets the bottom-left coordinate of this <see cref="Rect"/>.
		/// </summary>
		public Vector2d min {
			get {
				return new Vector2d(position.x, position.y + size.y);
			}
			set {
				position.x = value.x;
				size.y = value.y - position.y;
			}
		}

		/// <summary>
		/// Gets the top-right coordinate of this <see cref="Rect"/>.
		/// </summary>
		public Vector2d max {
			get {
				return new Vector2d(position.x + size.x, position.y);
			}
			set {
				position.y = value.y;
				size.x = value.x - position.x;
			}
		}

		/// <summary>
		/// Gets the left bound of this <see cref="Rect"/>.
		/// </summary>
		public double x {
			get {
				return position.x;
			}
			set {
				position.x = value;
			}
		}

		/// <summary>
		/// Gets the left bound of this <see cref="Rect"/>.
		/// </summary>
		public double xMin {
			get {
				return position.x;
			}
			set {
				position.x = value;
			}
		}

		/// <summary>
		/// Gets the right bound of this <see cref="Rect"/>.
		/// </summary>
		public double xMax {
			get {
				return position.x + size.x;
			}
			set {
				size.x = value - position.x;
			}
		}

		/// <summary>
		/// Gets the upper bound of this <see cref="Rect"/>.
		/// </summary>
		public double y {
			get {
				return position.y;
			}
			set {
				position.y = value;
			}
		}

		/// <summary>
		/// Gets the upper bound of this <see cref="Rect"/>.
		/// </summary>
		public double yMin {
			get {
				return position.y;
			}
			set {
				position.y = value;
			}
		}

		/// <summary>
		/// Gets the lower bound of this <see cref="Rect"/>.
		/// </summary>
		public double yMax {
			get {
				return position.y + size.y;
			}
			set {
				size.y = value - position.y;
			}
		}

		/// <summary>
		/// Creates a new <see cref="Rect"/> object.
		/// </summary>
		/// <param name="left">Left bound.</param>
		/// <param name="top">Upper bound.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public Rect(double left, double top, double width, double height) : this() {
			position = new Vector2d(left, top);
			size = new Vector2d(width, height);
		}

		/// <summary>
		/// Creates a new <see cref="Rect"/> object from the specified bounds.
		/// </summary>
		/// <param name="left">Left bound.</param>
		/// <param name="top">Upper bound.</param>
		/// <param name="right">Right bound.</param>
		/// <param name="bottom">Lower bound.</param>
		/// <returns>The resulting <see cref="Rect"/></returns>
		public static Rect MinMaxRect(double left, double top, double right, double bottom) {
			return new Rect(left, top, right - left, bottom - top);
		}

		/// <summary>
		/// Gets whether this <see cref="Rect"/> contains the specified point.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <param name="allowInverse">If the <see cref="Rect"/>'s size is negative, should the check allow this?</param>
		/// <returns><c>true</c> if the point is contained in the <see cref="Rect"/>.</returns>
		public bool Contains(Vector2d point, bool allowInverse = false) {
			if (allowInverse) {
				return point.x > position.x && point.x < position.x + size.x && point.y > position.y && point.y < position.y + size.y;
			} else {
				double xmax = Math.Max(x, xMax);
				double ymax = Math.Max(y, yMax);

				return MinMaxRect(x, y, xmax, ymax).Contains(point, true);
			}
		}

		/// <summary>
		/// Gets whether this <see cref="Rect"/> overlaps the specified <see cref="Rect"/>.
		/// </summary>
		/// <param name="other">The <see cref="Rect"/> to check.</param>
		/// <param name="allowInverse">If either <see cref="Rect"/>'s size is negative, should the check allow this?</param>
		/// <returns><c>true</c> if the other <see cref="Rect"/> overlaps this <see cref="Rect"/>.</returns>
		public bool Overlaps(Rect other, bool allowInverse = false) {
			if (allowInverse) {
				return Contains(other.position, true) || Contains(other.position + other.size, true) || Contains(other.min, true) || Contains(other.max, true) ||
				       other.Contains(position, true) || other.Contains(position + size, true) || other.Contains(min, true) || other.Contains(max, true);
			} else {
				double xmax = Math.Max(x, xMax);
				double ymax = Math.Max(y, yMax);
				double otherXmax = Math.Max(other.x, other.xMax);
				double otherYmax = Math.Max(other.y, other.yMax);

				return MinMaxRect(x, y, xmax, ymax).Overlaps(MinMaxRect(other.x, other.y, otherXmax, otherYmax), true);
			}
		}

		/// <summary>
		/// Sets this <see cref="Rect"/>'s components.
		/// </summary>
		/// <param name="left">The left bound to set.</param>
		/// <param name="top">The upper bound to set.</param>
		/// <param name="width">The width to set.</param>
		/// <param name="height">The height to set.</param>
		public void Set(double left, double top, double width, double height) {
			position.x = left;
			position.y = top;
			size.x = width;
			size.y = height;
		}

		/// <summary>
		/// Given a normalized point, returns a coordinate point inside the <see cref="Rect"/>.
		/// </summary>
		/// <param name="rect">The <see cref="Rect"/>.</param>
		/// <param name="normalizedCoordinates">The normalized coordinates.</param>
		/// <returns>The denormalized coordinates.</returns>
		public static Vector2d NormalizedToPoint(Rect rect, Vector2d normalizedCoordinates) {
			return new Vector2d(rect.x + (rect.width * normalizedCoordinates.x), rect.y + (rect.height * normalizedCoordinates.y));
		}

		/// <summary>
		/// Given a point, returns a normalized point relative to the <see cref="Rect"/>.
		/// </summary>
		/// <param name="rect">The <see cref="Rect"/>.</param>
		/// <param name="point">The coordinates.</param>
		/// <returns>The normalized coordinates.</returns>
		public static Vector2d PointToNormalized(Rect rect, Vector2d point) {
			if (rect.width != 0 && rect.height != 0) {
				return new Vector2d((point.x - rect.x) / rect.width, (point.y - rect.y) / rect.height);
			} else {
				return new Vector2d(Double.NaN, Double.NaN);
			}
		}

		/// <summary>
		/// Gives a nicely formatted <c>string</c> for this <see cref="Rect"/>.
		/// </summary>
		/// <returns>Nicely formatted <c>string</c> for this <see cref="Rect"/>.</returns>
		public override string ToString() {
			return string.Format("(x:{1}, y:{2}, width:{3}, height:{4})", x, y, width, height);
		}

		#region IEquatable
		/// <summary>
		/// Determines whether this <see cref="Rect"/> equals another.
		/// </summary>
		/// <param name="other">The <see cref="Rect"/> to compare to.</param>
		/// <returns><c>true</c> if the <see cref="Rect"/>s are equal.</returns>
		public bool Equals(Rect other) {
			return position == other.position && size == other.size;
		}

		/// <summary>
		/// Determines whether this <see cref="Rect"/> equals another <c>object</c>.
		/// </summary>
		/// <param name="other">The <c>object</c> to compare to.</param>
		/// <returns><c>true</c> if the other <c>object</c> is not null and a <see cref="Rect"/>, and the <see cref="Rect"/>s are equal.</returns>
		public override bool Equals(object obj) {
			if (object.ReferenceEquals(obj, null)) { return false; }
			if (!(obj is Rect)) { return false; }
			return Equals((Rect)obj);
		}

		/// <summary>
		/// Determines whether this <see cref="Rect"/> equals another.
		/// </summary>
		/// <param name="other">The <see cref="Rect"/> to compare to.</param>
		/// <returns><c>true</c> if the <see cref="Rect"/>s are equal.</returns>
		public static bool operator ==(Rect r1, Rect r2) {
			return r1.Equals(r2);
		}

		/// <summary>
		/// Determines whether this <see cref="Rect"/> does not equal another.
		/// </summary>
		/// <param name="other">The <see cref="Rect"/> to compare to.</param>
		/// <returns><c>true</c> if the <see cref="Rect"/>s are not equal.</returns>
		public static bool operator !=(Rect r1, Rect r2) {
			return !r1.Equals(r2);
		}

		/// <summary>
		/// Generates a hash code for this instance based on instance data.
		/// </summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode() {
			return position.GetHashCode() ^ size.GetHashCode();
		}
		#endregion

	}
}
#endif
