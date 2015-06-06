using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq.Dynamic;
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
            _entityProperties = typeof(TEntity).GetProperties();
        }

        public IQueryable<TEntity> Execute()
        {
            BuildMasterExpression();
            return _collection.Provider.CreateQuery<TEntity>(_masterExpression);
        }

        private void BuildMasterExpression()
        {
            var collection = _collection as IQueryable;
            var props = _queryDescriptor.GetType().GetProperties();
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<WhereAttribute>();
                var value = prop.GetValue(_queryDescriptor);
                if (attr != null && value != null)
                    collection = AddDynamicWhere(prop, collection);
            }
            _masterExpression = collection.Expression;
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<WhereAttribute>();
                if (attr == null)
                    _masterExpression = AddWhereExpressions(prop, _masterExpression);
            }
            _masterExpression = AddOrderExpressions(props, _masterExpression);
            _masterExpression = AddPagingExpressions(props, _masterExpression);
        }

        private IQueryable AddDynamicWhere(PropertyInfo prop, IQueryable queryable)
        {
            var whereClause = prop.GetCustomAttribute<WhereAttribute>().WhereClause;
            return queryable.Where(whereClause);
        }

        private Expression AddWhereExpressions(System.Reflection.PropertyInfo prop, Expression rootExpression)
        {
            var propValue = prop.GetValue(_queryDescriptor);
            if (propValue == null)
                return rootExpression;
            if (IsAPropertyOfEntity(prop.Name))
            {
                return AddWhereEquals(prop.Name, propValue, rootExpression);
            }
            var parts = prop.Name.Split('_');
            if (parts.Length >= 2)
            {
                if (parts.Length == 2)
                {
                    if (IsAPropertyOfEntity(parts[0]))
                        return AddWhereWithOperator(parts[0], (Op)Enum.Parse(typeof(Op), parts[1]), propValue, rootExpression);
                }
            }
            return rootExpression;
        }
        private Expression AddOrderExpressions(PropertyInfo[] props, Expression rootExpression)
        {
            var orderByProp = props.SingleOrDefault(prop => prop.Name == "OrderBy" && prop.PropertyType == typeof(string[]));
            if (orderByProp != null)
            {
                var orderByValue = orderByProp.GetValue(_queryDescriptor);
                if (orderByValue != null)
                    rootExpression = AddAllOrderBys((string[])orderByValue, rootExpression);
            }
            var orderByDescProp = props.SingleOrDefault(prop => prop.Name == "OrderByDesc" && prop.PropertyType == typeof(string[]));
            if (orderByDescProp != null)
            {
                var orderByDescValue = orderByDescProp.GetValue(_queryDescriptor);
                if (orderByDescValue != null)
                    rootExpression = AddAllOrderByDescs((string[])orderByDescValue, rootExpression);
            }
            return rootExpression;
        }
        private Expression AddPagingExpressions(PropertyInfo[] props, Expression rootExpression)
        {
            var skipProp = props.SingleOrDefault(prop => prop.Name == "Skip" && prop.PropertyType == typeof(int?));
            if (skipProp != null)
            {
                var skipValue = (int?)skipProp.GetValue(_queryDescriptor);
                if (skipValue.HasValue)
                    rootExpression = AddSkip(skipValue.Value, rootExpression);
            }

            var takeProp = props.SingleOrDefault(prop => prop.Name == "Take" && prop.PropertyType == typeof(int?));
            if (takeProp != null)
            {
                var takeValue = (int?)takeProp.GetValue(_queryDescriptor);
                if (takeValue.HasValue)
                    rootExpression = AddTake(takeValue.Value, rootExpression);
            }

            return rootExpression;
        }


        private Expression AddSkip(int skipCount, Expression rootExpression)
        {
            var skipExpr = Expression.Call(
                typeof(Queryable),
                "Skip",
                new[] { _collection.ElementType },
                rootExpression,
                Expression.Constant(skipCount));
            return skipExpr;
        }
        private Expression AddTake(int takeCount, Expression rootExpression)
        {
            var takeExpr = Expression.Call(
                typeof(Queryable),
                "Take",
                new[] { _collection.ElementType },
                rootExpression,
                Expression.Constant(takeCount));
            return takeExpr;
        }
        private Expression AddWhereWithOperator(string fieldName, Op op, object referenceValue, Expression rootExpression)
        {
            var itemExpr = Expression.Parameter(typeof(TEntity), "entity");
            var left = Expression.Property(itemExpr, fieldName);
            var right = Expression.Constant(referenceValue);
            Expression operationExpr;
            switch (op)
            {
                case Op.Lt:
                    operationExpr = Expression.LessThan(left, right);
                    break;
                case Op.Le:
                    operationExpr = Expression.LessThanOrEqual(left, right);
                    break;
                case Op.Gt:
                    operationExpr = Expression.GreaterThan(left, right);
                    break;
                case Op.Ge:
                    operationExpr = Expression.GreaterThanOrEqual(left, right);
                    break;
                case Op.Ne:
                    operationExpr = Expression.NotEqual(left, right);
                    break;
                case Op.Eq:
                    operationExpr = Expression.Equal(left, right);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("op");
            }
            var whereExpr = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { _collection.ElementType },
                rootExpression,
                Expression.Lambda<Func<TEntity, bool>>(operationExpr, new ParameterExpression[] { itemExpr })
                );
            return whereExpr;
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
        private bool IsAPropertyOfEntity(string propertyName)
        {
            return _entityProperties.Select(p => p.Name).Any(p => p == propertyName);
        }
    }
}
