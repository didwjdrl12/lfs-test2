using JNetwork;
using System;
using System.Collections.Generic;


//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// [����] �⺻ ����
//
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

public partial class MdBase
{
	public static eLanguage _language = eLanguage.Korean; // Korean, Chinese, English, Japanese

	
	// todo: ����
	public static int _port_main_server = 8401;
	public static int _port_field_server = 8402;
	public static float _position_send_duration = 0.1f;
	
	public static bool _network_encryption = false;
	//public static NetXtea _encryption_key = new NetXtea("temp24qw");


	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	// ���� ����
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public static int _max_hero_level = 100;
	public static int _max_room_slot = 6;
	public static int _max_skill_slot = 10;
	public static readonly int _max_dungeon_point = 120;
	public static readonly int _dungeon_point_get_duration = 5; //  5�п� �ϳ���?    
	public static readonly int _guild_create_gold = 5000;
    public static readonly int _max_stage_clear_ticket = 100;

    // �κ��丮
    public static int _max_inventory_pages = 4;
    public static int _max_inventory_lines_per_page = 5;
    public static int _max_inventory_slots_per_line = 5;
    public static readonly int _hero_dye_price = 30;
    public static readonly int _dice_instant_make_price = 100;
    public const int _max_inventory_slot = 80;
    
    // �ΰ���
    public static float _max_wait_room_time = 10f;
	public static float _max_game_time = 10f;



    public static readonly uint _skill_init_price = 30;
    
}

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// [����] const 
//
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

public static class cWeaponType
{
	public const int KnightSword = 21101;			// ���
	public const int WarriorSword = 21102;			// ����
	public const int HunterBow = 21103;				// ��ɲ�
	public const int SorcererStaff = 21104;			// ������
	public const int AlchemistDagger = 21105;		// ���ݼ���

	//public const int Axe = 21003;
}



