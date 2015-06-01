# LiteApi
ASP.NET Wrapper to easily write Restfulish Api Controllers

**Caution:** This project is in early stages of development.

## How to use
To use LiteApi, you must define 3 types and then inherit you controller from ˋLiteApiControllerˋ. These types are:

1. Dto
2. Entity
3. QueryDescriptor

All of these types can be simple classes and no need to inherit them from specific classes. LiteApiController maps Entity to Dto with ˋAutoMapperˋ. Users of controller see Dto but LiteApiController works with entity when it needs to work with persistence layer.

QueryDescriptor defines how users of controller can query collection of entity. 
