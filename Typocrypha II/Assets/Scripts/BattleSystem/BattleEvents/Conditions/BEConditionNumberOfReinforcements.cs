public class BEConditionNumberOfReinforcements : BEConditionNumberComparison
{
    protected override int Number => BattleManager.instance?.CurrWave?.reinforcementPrefabs.Count ?? 0;
}
