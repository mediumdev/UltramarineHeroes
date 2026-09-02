using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreUtils.Utils
{
	public static class RandomUtils {
		
		public interface IRandomWeight
		{
			float Weight();
		}
		
		[Serializable]
		public class RandomBaseWeight<T> : IRandomWeight
		{
			[SerializeField] private float _weight;
			[SerializeField] private T _item;
			
			public float Weight()
			{
				return _weight;
			}

			public T Item
			{
				get { return _item; }
			}
		}
		
		[Serializable]
		public class RandomIntWeight : RandomBaseWeight<int>{}
		[Serializable]
		public class RandomFloatWeight : RandomBaseWeight<float>{}
		[Serializable]
		public class RandomStringWeight : RandomBaseWeight<string>{}

		public static T GetRandom<T>(T[] items) where T : IRandomWeight
		{
			var weight = Random.Range(0f, 1f);
			var randoms = items.Where(x => x.Weight() > weight).ToArray();
			var item = items.FirstOrDefault();
			if (randoms.Length > 0)
			{
				item = randoms[Random.Range(0, randoms.Length)];
			}
			return item;
		}
	}
}
