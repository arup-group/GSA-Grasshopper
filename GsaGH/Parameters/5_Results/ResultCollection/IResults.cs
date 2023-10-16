namespace GsaGH.Parameters.Results {
  public interface IResults<IResult> {
    public IResultDictionary<IResultCollection<IResult>> ResultCache { get; set; }
    public IResultSubset<IResult> GetResultSet(string definition);
  } 
}
