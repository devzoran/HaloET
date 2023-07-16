using System.Collections.Generic;
using System.Linq;

namespace ET
{
	public class QueueDictionary<T, K>
	{
		private readonly List<T> list = new List<T>();
		private readonly Dictionary<T, K> dictionary = new Dictionary<T, K>();

		public void Enqueue(T t, K k)
		{
			this.list.Add(t);
			this.dictionary.Add(t, k);
		}
		
		/// <summary>
		/// 添加内容, 如果有重复的内容, 则会先移除旧数据,再重新添加
		/// </summary>
		/// <param name="t"></param>
		/// <param name="k"></param>
		public void ForceEnqueue(T t, K k)
		{
			if (this.dictionary.ContainsKey(t))
			{
				this.list.Remove(t);
				this.dictionary.Remove(t);
			}
			
			this.list.Add(t);
			this.dictionary.Add(t, k);
		}

		public K Dequeue()
		{
			if (this.list.Count == 0)
				return default;
			
			T t = this.list[0];
			this.list.RemoveAt(0);
			this.dictionary.TryGetValue(t, out K val);
			this.dictionary.Remove(t);
			return val;
		}
		
		public K Peek()
		{
			if (this.list.Count == 0)
				return default;

			T t = this.list[0];
			return this[t];
		}

		public void Remove(T t)
		{
			this.list.Remove(t);
			this.dictionary.Remove(t);
		}

		public bool ContainsKey(T t)
		{
			return this.dictionary.ContainsKey(t);
		}

		public bool TryGetValue(T t, out K k)
		{
			return dictionary.TryGetValue(t, out k);
		}
		
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public T FirstKey
		{
			get
			{
				if (this.list.Count == 0)
					return default;
				
				return this.list[0];
			}
		}
		
		public K FirstValue
		{
			get
			{
				if (this.list.Count == 0)
					return default;
				
				T t = this.list[0];
				return this[t];
			}
		}

		public T LastKey
		{
			get
			{
				if (this.list.Count == 0)
					return default;

				return this.list.Last();
			}
		}

		public K LastValue
		{
			get
			{
				if (this.list.Count == 0)
					return default;

				T t = this.LastKey;
				return this[t];
			}
		}
		
		public List<T> Keys
		{
			get
			{
				return this.list;
			}
		}
		public K[] Values
		{
			get
			{
				return dictionary.Values.ToArray();
			}
		}
		
		/// <summary>
		/// 仅供访问数据，请勿通过该接口移除或新增数据
		/// </summary>
		public Dictionary<T, K> KeyValues
		{
			get
			{
				return dictionary;
			}
		}
		
		public K this[T t]
		{
			get
			{
				return this.dictionary[t];
			}
		}

		public void Clear()
		{
			this.list.Clear();
			this.dictionary.Clear();
		}
	}
}