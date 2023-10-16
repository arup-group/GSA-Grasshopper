namespace GsaGH.Parameters.Results {
  public interface INodeResult : IResultCollection<IResult> { 
    public IResult Result { get; set; }
  } 
}
