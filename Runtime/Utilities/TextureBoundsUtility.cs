namespace NativeTexture.Utilities
{
	using Unity.Collections;

	/// <summary>
	/// Utility methods for working with texture value bounds and normalization.
	/// </summary>
	public static class TextureBoundsUtility
	{
		/// <summary>
		/// Creates a new value bounds reference initialized to extreme values for tracking.
		/// </summary>
		/// <param name="allocator">The allocator to use for the reference.</param>
		/// <returns>A NativeReference containing initialized value bounds.</returns>
		public static NativeReference<ValueBounds> CreateValueBounds(Allocator allocator)
		{
			NativeReference<ValueBounds> boundsRef = new(allocator);
			boundsRef.Reset();
			return boundsRef;
		}

		/// <summary>
		/// Sets explicit bounds values.
		/// </summary>
		/// <param name="boundsRef">The bounds reference to modify.</param>
		/// <param name="min">The minimum value to set.</param>
		/// <param name="max">The maximum value to set.</param>
		public static void SetExplicitBounds(
			NativeReference<ValueBounds> boundsRef,
			float min,
			float max
		)
		{
			ValueBounds bounds = boundsRef.Value;
			bounds.Min = min;
			bounds.Max = max;
			boundsRef.Value = bounds;
		}
	}
}
