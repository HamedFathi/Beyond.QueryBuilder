namespace Beyond.QueryBuilder.Models;

internal class OperatorInfo
{
    public bool AcceptsArrayAsSecondOperand { get; set; }
    public bool AcceptsSecondOperand { get; set; }
    public string Name { get; set; } = null!;
}