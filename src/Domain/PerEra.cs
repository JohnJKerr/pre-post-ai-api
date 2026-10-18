namespace Domain;

public record PerEra<T>(T Pre, T Post)
{
    public T this[Era era] => era == Era.Pre ? Pre : Post;
}
