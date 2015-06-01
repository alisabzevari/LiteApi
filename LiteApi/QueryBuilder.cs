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
        private Expression AddOrderByDesc(string fieldName, Expression rootExpression)
        {
            var itemExpr = Expression.Parameter(typeof(TEntity), "entity");
            var propertyToOrderByExpr = Expression.Property(itemExpr, fieldName);
            var orderByExpr = Expression.Call(
                typeof(Queryable),
                "OrderByDesc",
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
                _masterExpression = AddExpressionBasedOnQueryDescriptorPeroperty(prop, _masterExpression);
            }
        }

        private Expression AddExpressionBasedOnQueryDescriptorPeroperty(System.Reflection.PropertyInfo prop, Expression rootExpression)
        {
            if (prop.Name == "OrderBy" && prop.PropertyType == typeof(string[]))
            {
                return AddAllOrderBys((string[]) prop.GetValue(_queryDescriptor), rootExpression);
            }
            if (prop.Name == "OrderByDesc" && prop.PropertyType == typeof (string[]))
            {
                return AddAllOrderByDescs((string[]) prop.GetValue(_queryDescriptor), rootExpression);
            }
            return rootExpression;
        }

        private Expression AddAllOrderByDescs(string[] orderByFields, Expression rootExpression)
        {
            Expression allOrderByDescsExpr = rootExpression;
            foreach (var field in orderByFields)
            {
                allOrderByDescsExpr = AddOrderByDesc(field, allOrderByDescsExpr);
            }
            return allOrderByDescsExpr;
        }

        private Expression AddAllOrderBys(string[] orderByFields, Expression rootExpression)
        {
            Expression allOrderBysExpr = rootExpression;
            foreach (var field in orderByFields)
            {
                allOrderBysExpr = AddOrderBy(field, allOrderBysExpr);
            }
            return allOrderBysExpr;
        }
    }
}
