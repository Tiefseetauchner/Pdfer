namespace Pdfer.Objects;

public static class DocumentObjectSerializerRepositoryFactory
{
  public static IDocumentObjectSerializerRepository CreateForAllSerializers()
  {
    var repository = new DocumentObjectSerializerRepository();

    repository.AddSerializer(new ArrayObjectSerializer(repository));
    repository.AddSerializer(new DictionaryObjectSerializer(repository));
    repository.AddSerializer(new IndirectObjectSerializer());
    repository.AddSerializer(new NameObjectSerializer());
    repository.AddSerializer(new NumericObjectSerializer());
    repository.AddSerializer(new StreamObjectSerializer(repository));
    repository.AddSerializer(new StringObjectSerializer());

    return repository;
  }
}