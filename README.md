string extension helper method for format urls

  # example
```c#
                                         #example of anonymous type
 var request = new
 {
      Route_Id = 1,
      Name = "test"
 };

var url = "https://example.com/product/{id}/items".UrlFormat(request);
/* 
	return https://example.com/product/1/items?Name=test
*/

                                           # example of class type
										   
										   
#example 1
 public class ProducQueryDto
{
  [Route]
  public string Id { get; set; }
  public string Name { get; set; }
}
 
 var request = new ProducQueryDto
 {
    Id = 1,
    Name = "test"
 }
 
 var url = "https://example.com/product/{id}/items".UrlFormat(request);
/*  
     return  https://example.com/product/1/items?Name=test
*/

#example 2
public class ProducQueryDto
{
  [Route]
  public string Id { get; set; }
  [Query]
  public string Name { get; set; }
}
 
 var request = new ProducQueryDto
 {
    Id = 1,
    Name = "test"
 }
 
 var url = "https://example.com/product/{id}/items".UrlFormat(request);
/*  
		return https://example.com/product/1/items?Name=test
*/

#example 3
[DisableDefaultBehaviour]
public class ProducQueryDto
{
  [Route]
  public string Id { get; set; }
  public string Name { get; set; }
}
 
 var request = new ProducQueryDto
 {
    Id = 1,
    Name = "test"
 }
 
 var url = "https://example.com/product/{id}/items".UrlFormat(request);
/*  
	return https://example.com/product/1/items
*/

#example 4
public class ProducQueryDto
{
  [Route("id")]
  public string ProductId { get; set; }
  [Query("name")]
  public string ProductName { get; set; }
}
 
 var request = new ProducQueryDto
 {
    ProductId = 1,
    ProductName = "test"
 }
 
 var url = "https://example.com/product/{id}/items".UrlFormat(request);
/* 
		return https://example.com/product/1/items?name=test
*/
 
  ```
