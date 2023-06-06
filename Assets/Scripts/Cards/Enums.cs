namespace Cards
{
	public enum CardUnitType : byte
	{
		None = 0,
		Murloc = 1,
		Beast = 2,
		Elemental = 3,
		Mech = 4
	}

	public enum SideType : byte
	{
		Common = 0,
		Hunter = 1,
		Priest = 2,
		Mage = 3,
		Warrior = 4
	}

	public enum CardStateType : byte
    {
		InDeck,
		InHand,
		OnTable,
		InMenu,
		OnStartMenu
    }
	public enum FieldType :byte
    {
		Player1Hand,
		Player1Table,
		Player2Hand,
		Player2Table
    }

	public enum TypeByDescription
	{
		None,
		Taunt,
		Charge,
		Battlecry,
		Aura
	}
	public enum TypeOfBattlecry
    {
		None,
		DealDamage,
		RestoreHealths,
		Summon,
		DestroyWeapon,
		Buff,
		DrawCard
    }
	public enum TypeOfAura
    {
		None,
		Buff,
		Debuff,
		SpellDamage
    }
}
