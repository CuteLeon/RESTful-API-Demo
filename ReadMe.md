# RESTfull API

# 简介

​	2000年由 Roy Fielding 的论文中提出。

​	Representational State transfer 状态表述转换。

​	它描述了 Web 应用怎么设计才是优良的，定义了以下三点：

1. 一组网页的网络；=>一个虚拟机
2. 用户在网页上通过点击链接来前进；=>状态转换
3. 点击链接的结果就是传输了一个新的页面并渲染展示给用户；=>程序的下一个状态

# REST是一种架构风格

​	REST是一种架构风格，而不是规范或标准。

​	REST需要使用一些规范、协议或标准来实现这种风格。

​	REST与协议无关，HTTP 和 Json 也不是REST强制使用的。

# 优点

1. 通讯简单高效=>性能
2. 分布式系统=>组件交互的可扩转性
3. 关注点分离=>组件的可修改性
4. 语言无关、平台无关=>可移植性
5. 无状态约束，故障后轻松恢复=>可靠性
6. 无状态性为请求添加了完整的状态数据=>可视性

# 约束

## 客户端-服务器

​	 第一个约束是基于客户端-服务器架构背后的原则——关注点分离。通过分离用户界面和数据存储这两个关注点，提高了用户界面跨平台的可能性，通过简化服务器组件提高了其可伸缩性。这种分离对于Web来说更加重要的意义是，它使得组件能够进行独立修改和扩展，从而能够支持大量组织的网络化需求。 

## 无状态

​	服务端不需要记录客户端的状态，而由客户端维护自己的状态，客户端发送的请求包含所需要的的所有状态信息，轻松实现跨服务调用。

## 统一资源接口/界面

​	 统一接口约束是设计任何REST服务的基础。它简化和解耦的架构，使得每个部分可以独立被修改。 

### 资源标识

​	各种资源都在请求中被确定，例如在使用URI的Web REST系统中，资源本身就表明它们想要被返回给客户端的格式。

### 通过表述来对资源进行操作

​	 当一个客户端持有一种资源的表述时，包括任何关联的元数据，它都有足够的信息来修改或删除该资源。 

### 带有自我描述信息

​	 每条信息都包含足够的信息来描述如何处理消息本身。 

### 超媒体作为应用程序状态的引擎 (HATEOAS)

​	 已经初始化URI的REST客户端应用，应该就像人们访问了一个Web网站主页一样，能够使用服务器提供的动态链接，请求它所需的资源和进行所需的操作。 

## 多层系统

​	 客户端通常无法判断它是否是直接连接到后端服务器，还是中间服务器。中间服务器可通过启用负载平衡，并通过提供共享高速缓存来提高系统的可扩展性。当然也可以强制执行安全政策。

## 可缓存

​	 互联网中的客户端和中间层服务器可以缓存响应。因此响应必须直接或间接定义自身是否可被缓存，以免客户端使用过期的响应数据来发送其它请求。良好的缓存策略可以有效减少客户端-服务器之间的交互，从而进一步提高系统的可伸缩性和性能。 

## 按需编码（可选）

​	 服务端可以通过传递可执行代码临时为客户端扩展或自定义功能。 

# Richardson成熟度模型

## Level_0：POX(Plain old xml)沼泽

​	 该模型的出发点是使用HTTP作为远程交互的传输系统，但是不会使用Web中的任何机制。 仅仅将请求数据封装为XML或JSON发送到特定的地址，并接收返回，例如：

```
post: /BuyBook
request:
<xml><book name="CSharpBook"/></xml>
```

## Level_1：资源

​	不再与特定的接口通信，而是与资源通信；从思想上讲，Level_0就像是将一个对象作为参数传入一个方法，而Level_1更像是直接访问一个对象上的方法。

```
post /Book/CSharpBook
```

## Level_2：HTTP动词

​	合理使用HTTP的动词，并返回对应的状态代码；可以对同一个URI使用不同的HTTP动作执行不同的操作。

- GET [QueryString] => 获取资源
- POST [Body] => 创建资源
- PUT [Body] => 替换资源（使用客户端发送的全部数据）传输完整的资源数据，如果资源不存则自动创建
- PATCH [Body] => 更新资源（使用客户端发送的部分数据）传输需要更新的字段及数据的键值对
- DELETE [QueryString] => 删除资源

```
get /Books?name=CSharpBook
post /Book/CSharpBook?count=1
Response:
<xml><order name="CSharpBook"/></xml>
```

## Level_3：超媒体

​	HATEOAS(Hypertext As The Engine Of Application State) 超媒体作为程序状态引擎。返回的响应信息包含下一步的链接，指示程序下一步可以做什么以及响应的资源URI。可以保证不修改客户端的情况下快速修改资源的访URI方案。

```
get /Books?name=CSharpBook
post /Book/CSharpBook?count=1
Response:
<xml>
	<order name="CSharpBook">
		<link rel="/linkrels/Order"
			  uri="/Order/202003080020"/>
	</order>
</xml>
```

​	Level 1 解释了如何通过分治法(Divide and Conquer)来处理复杂问题，将一个大型的服务端点(Service Endpoint)分解成多个资源。

​	Level 2 引入了一套标准的动词，用来以相同的方式应对类似的场景，移除不要的变化。

​	Level 3 引入了可发现性(Discoverability)，它可以使协议拥有自我描述(Self-documenting)的能力。

# 对外合约

## API消费者的三个概念

1. 资源的标识
2. HTTP方法
3. 有效载荷（Payload）（可选）

## 使用名称，而非动词

​	使用名词访问资源，使用HTTP动词描述动作。

```
错误：/api/getusers
URI里出现了get动词；
正确：get /api/user
应该使用HTTP动词GET表述；
```

## URI可读

​	就像变量命名规范一样，URI应尽量友好且简短。

## 要体现资源的结构和关系

​	使API具有可预测性和一致性。

```
get /companies/{companyId}
get /companies/{companyid}/employees
get /companies/{companyid}/employees/{employessId}
```

## 自定义查询

​	URI内只可包含名词，所以一些自定义查询参数可以在QueryString中。

```
get /api/users?orderby=name&count=20
```

# 状态码

​	请求是否成功了？

​	如果失败了，谁将为失败负责？

## 1XX

​	信息性状态码，Web API 并不使用。

## 2XX

​	执行成功。

​	200：OK 执行成功

​	201：Created 创建资源成功

​	204：NoContent 执行成功但不应该返回任何结果，例如删除操作

## 3XX

​	重定向

## 4XX

​	客户端错误。

​	400：BadRequest 请求错误

​	401：Unauthorized 没有提供授权信息或授权信息错误

​	403：Forbidden 身份认证成功，但仍禁止访问此资源

​	404：NotFound 请求的资源不存在

​	405：MethodNotAllowed 使用不被支持的HTTP方法发送请求到资源

​	406：NotAcceptable API消费者的表述格式并不被WebAPI所支持，且API不会提供默认的表述格式

​	409：Conflict 请求与服务器当前状态冲突。通常指更新资源时发生的冲突，用来处理并发问题的状态码

​	415：UnsupportedMediaType API不支持媒体类型

​	422：UnprocessableEntity 服务器理解媒体类型但是仍无法处理此实体数据，用于表示实体数据验证的错误

## 5XX

​	服务器的故障

# 错误

​	通常有API消费者引起的Error，API消费者传输的数据不合理，API就回正常的拒绝请求并返回 4XX 状态代码。并不影响API的可用性。

# 故障

​	合理的请求仍无法被响应，由服务端本身引起，返回5XX的状态代码。会对API整体可用性造成影响。

# 内容协商

​	RESTful API 并不强制约束使用 JSON，只是JSON较为常用。内容协商时，针对一个响应，当有多种表述格式可用时，选取最佳的一个表述。

## 声明请求的表述格式

​	API消费者发送的请求头 Content-Type Header 用于声明请求附带数据的表述格式，例如 "application/json" 或 "application/xml"。

## 要求响应的表述格式

​	当不同的API消费者需要使用不同的表述格式时，可以在API消费者发送的请求头 AcceptHeader 里的 MediaType(媒体类型) 值设置为 "application/json" 或 "application/xml" 以要求API返回对应表述格式的数据。

​	当不指定媒体类型时，API可以返回默认的表述格式的数据。

​	当指定的媒体类型不被服务端理解时，服务端可以返回 406 错误代码，或者使用默认表述格式的数据。

​	ASP.NET Core 对应的是 OutPutFormatters。

# 数据模型

​	模型分离为不同的类型，可以使系统更加健壮、可靠、易于进化。将数据库和API消费者同时对数据库的依赖解耦。

## 实体模型-Entity Model

​	Entity Framework 使用的数据模型，用于数据库交互；

## 外部模型-DTO (Data Transfer Object)

​	数据交换模型，用于沟通外部和内部的数据模型；

​    应该为增、删、查三种场景实现不同成员的DTO模型，以便于在业务发展和变化过程中灵活的调整。

# ActionResult\<T\>

​	使用 ActionResult\<T\> 代替 IActionResult ，以明确API的响应数据类型；

# 父子关系的资源

​	在路由特性配置出父子资源的ID即可。

# 处理故障

​	Try {...} Catch 和 throw 对性能影响比较大。

​	当项目的 ASPNETCORE_ENVIRONMENT 环境变量值为 "Development" 时，API 抛出的异常将会发送给客户端，这是无用但危险的，将此环境变量改为 "Production" 即生产环境即可。

​	可以使用 app.UseExceptionHandler(appBuilder) 配置全局异常处理表达式；

# HTTP HEAD

​	Head 和 Get 几乎一样，但是 Head 不应该返回响应的 Body，即 Head 没有 Payload。Head 可以获取资源的一些信息。

# HTTP OPTIONS

​	由API消费者用于获取针对某个WebAPI的通信选项的信息。

# 过滤和搜索

​	过滤和搜索的条件参数不属于资源。

## 如何给API传输数据

​	在 ASP.NET CORE 可以通过特性修饰参数来声明参数从哪里取值。

- [FromBody]：请求的Body => 推断复杂类型参数
- [FromForm]：请求的Body的Form表单 => 推断 IFormFile 和 IFormFileCollection
- [FromHeader]：请求的Header 
- [FromQuery] ： 请求的Query字符串 => 推断路由模板中和参数名称一致的数据
- [FromRoute] ： 请求的路由数据 => 推断其他参数
- [FromService] ： 依赖注入的服务

## 过滤

​	使用限定条件返回过滤后的数据集合。将需要过滤的字段名称及过滤的值一起传给API，将匹配/不匹配的数据移除，过滤条件之间为"&&"关系；

## 搜索

​	根据预定义的规则，将符合条件的数据添加到集合里。搜索条件之间为"||"关系。

# HTTP方法的安全性和幂等性

​	安全性是指方法执行后并不会改变资源的表述；

​	幂等性是指方法无论执行多少次都会得到相同的结果；

| HTTP方法 | 安全性 | 幂等性 |
| -------- | ------ | ------ |
| GET      | 是     | 是     |
| OPTIONS  | 是     | 是     |
| HEAD     | 是     | 是     |
| POST     | 不     | 不     |
| DELETE   | 不     | 是     |
| PUT      | 不     | 是     |
| PATCH    | 不     | 不     |

> Patch 不是幂等的：加入一个集合每次Patch后追加记录，则不满足幂等性；

# 输入验证和DataAnnotations

## 定义验证规则

​	常用的三种输入数据验证规则定义方法：

### DataAnnotations

​	应用 System.ComponentModel.DataAnnotations 名称空间，并使用其中的特性修饰模型中的属性；

### 自定义Attribute

​	增加模型验证特性类继承自 ValidationAttribute 并编写 IsValid 方法内的验证逻辑；

### 实现IValidatableObject接口

​	模型类继承并实现接口 IValidatableObject，并在 Validate 方法中编写自定义的验证逻辑，将验证错误通过 yield return 返回；

## 按验证规则进行检查

​	ModelState 对象是一个字典，既包含Model的状态，也包含Model的绑定验证信息。也包含针对每个提交的属性值的错误信息的集合。每当有请求进来的时候，定义好的验证规则就会被检查。

​	如果属性类型错误或验证不通过 ModelState.IsValid 就回返回 False。

​	验证错误返回的状态代码：422-UnprocessableEntity

## 报告验证的错误

​	[ApiController] 特性会自动完成以下逻辑：

```csharp
if (!this.ModelState.IsValid)
{
	return this.UnprocessableEntity(this.ModelState);
}
```

# 如果客户端负责创建资源

​	如果允许客户端创建资源，客户端可以生成URI，并使用PUT方法发送一个不对应任何资源的URI上。服务端在处理上述PUT请求时，如果对应的资源不存在，则创建资源；如果资源存在，则更新资源。所以PUT此时可以同时处理创建和替换；

# HTTP PATCH

​	局部更新。

## JSON PATCH Operations

### Add

```json
{ "op": "add", "path": "/biscuits/1", "value": { "name": "Ginger Nut" } }
```

### Remove

```json
{ "op": "remove", "path": "/biscuits" }
{ "op": "remove", "path": "/biscuits/0" }
```

### Move

```json
{ "op": "move", "from": "/biscuits", "path": "/cookies" }
```

### Replace

```json
{ "op": "replace", "path": "/biscuits/0/name", "value": "Chocolate Digestive" }
```

### Copy

```json
{ "op": "copy", "from": "/bizcuits/0", "path": "/biscuits/1" }
```

### Test

```json
{ "op": "test", "path": "/best_biscuit/name", "value": "cChoco Leibniz" }
```
