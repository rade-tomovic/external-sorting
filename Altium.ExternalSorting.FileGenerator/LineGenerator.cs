using Bogus.DataSets;

namespace Altium.ExternalSorting.FileGenerator;

public class LineGenerator
{
    private readonly Func<string> _contentGenerator;
    private int _orderNumber;
    private string _randomString = null!;

    public LineGenerator(Func<string>? contentGenerator = null)
    {
        _contentGenerator = contentGenerator ?? (() => new Lorem().Sentence());
        WithOrderNumber();
        WithRandomString();
    }

    private void WithOrderNumber()
    {
        _orderNumber = new Random().Next();
    }

    private void WithRandomString()
    {
        _randomString = _contentGenerator.Invoke();
    }

    public string Build()
    {
        return $"{_orderNumber}. {_randomString}";
    }
}