
public class ChunkNoise : SimplexNoise
{
	public ChunkNoise(int seed)
		: base(seed)
	{
	}

	public void FillMap2D(float[,] map, int cx, int cz, int octaves, float startFrequency, float startAmplitude)
	{
		const double DIVISOR = 1d / 16;

		int width = map.GetLength(0);

		for (int rx = 0; rx < width; rx++)
		{
			for (int rz = 0; rz < width; rz++)
			{
				double x = (rx + cx * width) * DIVISOR;
				double z = (rz + cz * width) * DIVISOR;

				double frequency = startFrequency;
				double amplitude = startAmplitude;

				double sum = 0;

				for (int octave = 0; octave < octaves; octave++)
				{
					double noise = GetNoise(x * frequency, z * frequency);

					sum += amplitude * noise;
					frequency *= 2;
					amplitude *= 0.5;
				}
				map[rx, rz] = (float)sum;
			}
		}

		return;
	}

	public void FillMap3D(float[, ,] map, int cx, int cz, int octaves, float startFrequency, float startAmplitude)
	{
		const double DIVISOR = 1d / 16;

		int width = map.GetLength(0);
		int height = map.GetLength(1);

		for (int rx = 0; rx < width; rx++)
		{
			double x = (rx + cx * width) * DIVISOR;

			for (int rz = 0; rz < width; rz++)
			{
				double z = (rz + cz * width) * DIVISOR;

				for (int ry = 0; ry < height; ry++)
				{
					double y = ry * DIVISOR;

					double frequency = startFrequency;
					double amplitude = startAmplitude;

					double sum = 0;

					for (int octave = 0; octave < octaves; octave++)
					{
						double noise = GetNoise(x * frequency, y * frequency, z * frequency);

						sum += amplitude * noise;
						frequency *= 2;
						amplitude *= 0.5;
					}
					map[rx, ry, rz] = (float)sum;
				}
			}
		}

		return;
	}

	public float GetValue3D(int wx, int wy, int wz, int octaves, float startFrequency, float startAmplitude)
	{
		const double DIVISOR = 1d / 16;

		double x = wx * DIVISOR;
		double y = wy * DIVISOR;
		double z = wz * DIVISOR;

		double frequency = startFrequency;
		double amplitude = startAmplitude;

		double sum = 0;

		for (int octave = 0; octave < octaves; octave++)
		{
			double noise = GetNoise(x * frequency, y * frequency, z * frequency);

			sum += amplitude * noise;
			frequency *= 2;
			amplitude *= 0.5;
		}

		return (float)sum;
	}
}
