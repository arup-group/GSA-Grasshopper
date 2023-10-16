namespace GsaGH.Parameters.Results {
  public interface ITri6Result : ITriResult<IResult> { 
    public IResult AB { get; set; }
    public IResult BC { get; set; }
    public IResult CA { get; set; }
  } 
}
