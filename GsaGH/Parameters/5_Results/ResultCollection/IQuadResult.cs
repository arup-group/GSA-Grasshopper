namespace GsaGH.Parameters.Results {
  public interface IQuadResult<IResult> : ITriResult<IResult> { 
    public IResult D { get; set; }
  } 
}
