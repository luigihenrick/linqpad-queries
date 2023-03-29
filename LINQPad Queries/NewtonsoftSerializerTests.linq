<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>System.Text.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

void Main()
{
	List<Fruit> fruits = new List<Fruit>
	{
		new Banana() { Name = null, Taste = null },
		new Apple() { Name = nameof(Apple), Taste = "sweet", Description = "Teste" },
	};
	
	var serializedFruits = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(fruits.Cast<Fruit>().ToList(), new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals  });
	var serializedFruitsNewtonsoft = JsonConvert.SerializeObject(fruits);
	
	(fruits.First() is Banana).Dump();
	(fruits.Last() is Apple).Dump();

	System.Text.Json.JsonSerializer.Deserialize<List<Fruit>>(serializedFruits).Dump();
	System.Text.Json.JsonSerializer.Deserialize<List<Fruit>>(serializedFruitsNewtonsoft).Dump();
}

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class Fruit 
{
	private string? description;
	
	public virtual string Name { get; set; }
	public virtual string Taste { get; set; }

	public virtual string Description { get => description ?? Name + Taste; set => description = value; }
}

[JsonObject(ItemNullValueHandling = NullValueHandling.Include)]
public class Banana : Fruit
{
}

[JsonDerivedType(typeof(Fruit))]
public class Apple : Fruit
{
	[JsonProperty(NullValueHandling = NullValueHandling.Include)]
	public override string Name { get; set; }
	[JsonProperty(NullValueHandling = NullValueHandling.Include)]
	public override string Description => base.Description;
}