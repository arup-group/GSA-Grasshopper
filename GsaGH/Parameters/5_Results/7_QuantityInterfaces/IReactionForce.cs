using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public interface IReactionForce : IResultVector6<Force?, Moment?>, IResultItem {
    Force? XToUnit(ForceUnit unit);
    Force? YToUnit(ForceUnit unit);
    Force? ZToUnit(ForceUnit unit);
    Force? XyzToUnit(ForceUnit unit);
    Moment? XxToUnit(MomentUnit unit);
    Moment? YyToUnit(MomentUnit unit);
    Moment? ZzToUnit(MomentUnit unit);
    Moment? XxyyzzToUnit(MomentUnit unit);
    double? XAs(ForceUnit unit);
    double? YAs(ForceUnit unit);
    double? ZAs(ForceUnit unit);
    double? XyzAs(ForceUnit unit);
    double? XxAs(MomentUnit unit);
    double? YyAs(MomentUnit unit);
    double? ZzAs(MomentUnit unit);
    double? XxyyzzAs(MomentUnit unit);
  }
}
