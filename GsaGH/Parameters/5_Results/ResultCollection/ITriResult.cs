namespace GsaGH.Parameters.Results {
  public interface ITriResult<IResult> : IElement2dResult<IResult>{ 
    public IResult A { get; set; }
    public IResult B { get; set; }
    public IResult C { get; set; }
  } 
}
