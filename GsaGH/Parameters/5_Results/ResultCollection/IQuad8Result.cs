namespace GsaGH.Parameters.Results {
  public interface IQuad8Result<IResult> : IQuadResult<IResult> {
    public IResult AB { get; set; }
    public IResult BC { get; set; }
    public IResult CD { get; set; }
    public IResult DA { get; set; }
  } 
}
