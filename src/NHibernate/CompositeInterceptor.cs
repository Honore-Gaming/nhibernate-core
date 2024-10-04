using System.Collections.Generic;
using System.Linq;
using NHibernate.SqlCommand;

namespace NHibernate
{
	public class CompositeInterceptor : EmptyInterceptor
	{
		private readonly List<IInterceptor> interceptors;

		public CompositeInterceptor() : this(null)
		{
		}

		public CompositeInterceptor(params IInterceptor[] interceptors)
		{
			this.interceptors = interceptors?.ToList() ?? new List<IInterceptor>();
		}

		public void AddInterceptor(IInterceptor interceptor)
		{
			this.interceptors.Add(interceptor);
		}

		public void InsertInterceptor(int index, IInterceptor interceptor)
		{
			this.interceptors.Insert(index, interceptor);
		}

		public override SqlString OnPrepareStatement(SqlString sql)
		{
			SqlString currentSql = sql;
			foreach (var interceptor in this.interceptors)
			{
				currentSql = interceptor.OnPrepareStatement(currentSql);
			}

			return currentSql;
		}

		public override void SetSession(ISession session)
		{
			foreach (var interceptor in this.interceptors)
			{
				interceptor.SetSession(session);
			}
		}
	}
}
