using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiteApi
{
    public class QueryBuilder<TEntity, TQueryDescriptor>
    {
        private readonly IQueryable<TEntity> _collection;
        private readonly TQueryDescriptor _queryDescriptor;
        private Expression _masterExpression;
        private readonly PropertyInfo[] _entityProperties;

        public QueryBuilder(IQueryable<TEntity> collection, TQueryDescriptor queryDescriptor)
        {
            _queryDescriptor = queryDescriptor;
            _collection = collection;
            _entityProperties = typeof (TEntity).GetProperties();
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
            var propValue = prop.GetValue(_queryDescriptor);
            if (propValue == null)
                return rootExpression;
            if (prop.Name == "OrderBy" && prop.PropertyType == typeof(string[]))
            {
                return AddAllOrderBys((string[]) propValue, rootExpression);
            }
            if (prop.Name == "OrderByDesc" && prop.PropertyType == typeof(string[]))
            {
                return AddAllOrderByDescs((string[])propValue, rootExpression);
            }
            if (_entityProperties.Select(p => p.Name).Any(p => p == prop.Name))
            {
                return AddWhereEquals(prop.Name, propValue, rootExpression);
            }
            return rootExpression;
        }

        private Expression AddWhereEquals(string fieldName, object referenceValue, Expression rootExpression)
        {
            var itemExpr = Expression.Parameter(typeof(TEntity), "entity");
            //var propertyToOrderByExpr = Expression.Property(itemExpr, fieldName);
            var left = Expression.Property(itemExpr, fieldName);
            var right = Expression.Constant(referenceValue);
            var eq = Expression.Equal(left, right);

            var whereExpr = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { _collection.ElementType },
                rootExpression,
                Expression.Lambda<Func<TEntity, bool>>(eq, new ParameterExpression[] { itemExpr })
                );
            return whereExpr;
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
