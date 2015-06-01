using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LiteApi
{
    public class QueryBuilder<TEntity, TQueryDescriptor>
    {
        private readonly IQueryable<TEntity> _collection;
        private readonly TQueryDescriptor _queryDescriptor;
        private Expression _masterExpression;

        public QueryBuilder(IQueryable<TEntity> collection, TQueryDescriptor queryDescriptor)
        {
            _queryDescriptor = queryDescriptor;
            _collection = collection;
        }

        private Expression AddOrderBy(string fieldName, Expression rootExpression)
        {
            var itemExpr = Expression.Parameter(typeof(TEntity), "entity");
            var propertyToOrderByExpr = Expression.Property(itemExpr, fieldName);
            var orderByExpr = Expression.Call(
                typeof(Queryable),
                "OrderBy",
                new[] { _collection.ElementType, typeof(IComparable) },
                rootExpression,
                Expression.Lambda<Func<TEntity, IComparable>>(propertyToOrderByExpr, new ParameterExpression[] { itemExpr })
                );
            return orderByExpr;
        }

        public IQueryable<TEntity> Execute()
        {
            BuildMasterExpression();
            return _collection.Provider.CreateQuery<TEntity>(_masterExpression);
        }

        private void BuildMasterExpression()
        {
            _masterExpression = _collection.Expression;
            var props = _queryDescriptor.GetType().GetProperties();
            foreach (var prop in props)
            {
                _masterExpression = BuildExpressionBasedOnQueryDescriptorPeroperty(prop, _masterExpression);
            }
        }

        private Expression BuildExpressionBasedOnQueryDescriptorPeroperty(System.Reflection.PropertyInfo prop, Expression rootExpression)
        {
            if (prop.Name == "OrderBy" && prop.PropertyType == typeof(string[]))
            {
                return AddAllOrderBys((string[]) prop.GetValue(_queryDescriptor), rootExpression);
            }
            return rootExpression;
        }

        private Expression AddAllOrderBys(string[] orderByFields, Expression rootExpression)
        {
            Expression AllOrderBysExpr = rootExpression;
            foreach (var field in orderByFields)
            {
                AllOrderBysExpr = AddOrderBy(field, AllOrderBysExpr);
            }
            return AllOrderBysExpr;
        }
    }
}
