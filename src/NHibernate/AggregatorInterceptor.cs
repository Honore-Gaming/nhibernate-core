using System.Collections.Generic;
using System.Linq;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate
{
	public class AggregatorInterceptor : EmptyInterceptor
	{
		private readonly List<IInterceptor> interceptors;

		public AggregatorInterceptor() : this(null)
		{
		}

		public AggregatorInterceptor(params IInterceptor[] interceptors)
		{
			this.interceptors = interceptors?.ToList() ?? new List<IInterceptor>();
		}

		public AggregatorInterceptor AddInterceptor(IInterceptor interceptor)
		{
			this.interceptors.Add(interceptor);
			return this;
		}

		public AggregatorInterceptor InsertInterceptor(int index, IInterceptor interceptor)
		{
			this.interceptors.Insert(index, interceptor);
			return this;
		}

		public override SqlString OnPrepareStatement(SqlString sql)
		{
			foreach (var interceptor in this.interceptors)
			{
				sql = interceptor.OnPrepareStatement(sql);
			}

			return sql;
		}

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			return this.interceptors.All(x => x.OnSave(entity, id, state, propertyNames, types));
		}

		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			return this.interceptors.All(x => x.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types));
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
