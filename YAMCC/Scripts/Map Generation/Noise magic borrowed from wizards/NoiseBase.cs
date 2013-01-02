using System;

public abstract class NoiseBase
{
	private const double DOUBLE_PI = 2 * Math.PI;

	protected Random random;

	/// <summary>
	///   Create 4D simplex noise generator.
	/// </summary>
	/// <param name = "seed">0 - random seed</param>
	protected NoiseBase(int seed)
	{
		random = seed == 0 ? new Random() : new Random(seed);
	}

	public virtual void Randomize()
	{
		random = new Random();
	}

	public abstract double GetNoise(double x, double y);
	public abstract double GetNoise(double x, double y, double z);
	public abstract double GetNoise(double x, double y, double z, double w);

	public double GetSeamlessNoise(double frequency, double x)
	{
		double angle = x * DOUBLE_PI;
		double xx = frequency * Math.Sin(angle) / DOUBLE_PI;
		double yy = frequency * Math.Cos(angle) / DOUBLE_PI;

		return GetNoise(xx, yy);
	}

	public double GetSeamlessNoise(double frequency, double x, double y)
	{
		double angle1 = x * DOUBLE_PI;
		double xx = frequency * Math.Sin(angle1) / DOUBLE_PI;
		double yy = frequency * Math.Cos(angle1) / DOUBLE_PI;

		double angle2 = y * DOUBLE_PI;
		double zz = frequency * Math.Sin(angle2) / DOUBLE_PI;
		double ww = frequency * Math.Cos(angle2) / DOUBLE_PI;

		return GetNoise(xx, yy, zz, ww);
	}

	public float[,] CreateSimpleMap(int resolution, int octaves = 8, float startFrequency = 1, float startAmplitude = 1, bool seamless = false)
	{
		double divisor = 1d / resolution;

		var map = new float[resolution, resolution];
		for (int rx = 0; rx < resolution; rx++)
		{
			for (int ry = 0; ry < resolution; ry++)
			{
				double x = rx * divisor;
				double y = ry * divisor;

				double frequency = startFrequency;
				double amplitude = startAmplitude;

				double sum = 0;

				for (int octave = 0; octave < octaves; octave++)
				{
					double noise;
					if (seamless)
					{
						noise = GetSeamlessNoise(frequency, x, y);
					}
					else
					{
						noise = GetNoise(x * frequency, y * frequency);
					}
					sum += amplitude * noise;
					frequency *= 2;
					amplitude *= 0.5;
				}
				map[rx, ry] = (float)sum;
			}
		}

		return map;
	}

	public float[,] CreateTurbilenceMap(int resolution, int octaves = 8, float startFrequency = 1, float startAmplitude = 1, bool seamless = false)
	{
		double divisor = 1d / resolution;

		var map = new float[resolution, resolution];
		for (int rx = 0; rx < resolution; rx++)
		{
			for (int ry = 0; ry < resolution; ry++)
			{
				double x = rx * divisor;
				double y = ry * divisor;

				double frequency = startFrequency;
				double amplitude = startAmplitude;

				double sum = 0;

				for (int octave = 0; octave < octaves; octave++)
				{
					double noise;
					if (seamless)
					{
						noise = GetSeamlessNoise(frequency, x, y);
					}
					else
					{
						noise = GetNoise(x * frequency, y * frequency);
					}
					sum += amplitude * Math.Abs(noise);
					frequency *= 2;
					amplitude *= 0.5;
				}
				map[rx, ry] = (float)sum;
			}
		}

		return map;
	}

	public float[,] CreateSinusMap(int resolution, int octaves = 8, float startFrequency = 1, float startAmplitude = 1, bool seamless = false)
	{
		double divisor = 1d / resolution;

		var map = new float[resolution, resolution];
		for (int rx = 0; rx < resolution; rx++)
		{
			for (int ry = 0; ry < resolution; ry++)
			{
				double x = rx * divisor;
				double y = ry * divisor;

				double frequency = startFrequency;
				double amplitude = startAmplitude;

				double sum = 0;

				for (int octave = 0; octave < octaves; octave++)
				{
					double noise;
					if (seamless)
					{
						noise = GetSeamlessNoise(frequency, x, y);
					}
					else
					{
						noise = GetNoise(x * frequency, y * frequency);
					}
					sum += amplitude * Math.Abs(noise);
					frequency *= 2;
					amplitude *= 0.5;
				}
				map[rx, ry] = (float)(Math.Sin(x * DOUBLE_PI + sum));
			}
		}
		return map;
	}
}
