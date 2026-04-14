namespace Complex.Application.Common.Libs;

public static class GeoMath
{
	public static decimal BearingDegrees(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
	{
		// Converte para radianos
		double φ1 = DegreesToRadians((double)lat1);
		double φ2 = DegreesToRadians((double)lat2);
		double Δλ = DegreesToRadians((double)(lon2 - lon1));

		double y = Math.Sin(Δλ) * Math.Cos(φ2);
		double x = Math.Cos(φ1) * Math.Sin(φ2)
			- Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(Δλ);

		double θ = Math.Atan2(y, x); // -π..π
		double brng = (RadiansToDegrees(θ) + 360d) % 360d;

		return (decimal)brng;
	}

	private static double DegreesToRadians(double deg) => deg * Math.PI / 180d;
	private static double RadiansToDegrees(double rad) => rad * 180d / Math.PI;
}
