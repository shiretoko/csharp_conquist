using System;
using System.Collections.Generic;
using System.Linq;

class Area
{
    public string Name;
    public int OwnerTeam;  // 0 = ไม่มี, 1 = human, 2 = AI ...
    public int Troops;

    public Area(string name)
    {
        Name = name;
        OwnerTeam = 0;
        Troops = 0;
    }

    public override string ToString()
    {
        return $"[{Name} ({OwnerTeam}:{Troops})]";
    }
}

class Player
{
    public string Name;
    public bool IsHuman;
    public List<string> Areas;

    public Player(string name, bool isHuman)
    {
        Name = name;
        IsHuman = isHuman;
        Areas = new List<string>();
    }

    public string DisplayName(int humanCount)
    {
        if(IsHuman)
        {
            return humanCount == 1 ? "You" : Name;
        }
        return Name + " (AI)";
    }
}

class Program
{
    static void Main()
    {
        Random rand = new Random();

        // 1. สร้างพื้นที่ A-L
        string[] areaNames = { "A","B","C","D","E","F","G","H","I","J","K","L" };
        Area[] map_area = areaNames.Select(n => new Area(n)).ToArray();

        // 2. เลือกจำนวนผู้เล่น
        Console.WriteLine("จำนวนผู้เล่นมนุษย์ (1-5): ");
        int humanCount = int.Parse(Console.ReadLine());
        Console.WriteLine("จำนวน AI (0-4, รวมไม่เกิน 5): ");
        int aiCount = int.Parse(Console.ReadLine());
        int totalPlayers = humanCount + aiCount;
        if(totalPlayers > 5) totalPlayers = 5;

        // 3. สร้างผู้เล่น
        List<Player> players = new List<Player>();
        for(int i=1;i<=humanCount;i++) players.Add(new Player($"user{i}", true));
        for(int i=humanCount+1;i<=totalPlayers;i++) players.Add(new Player($"user{i}", false));

        // 4. สุ่มลำดับผู้เล่น
        players = players.OrderBy(p => rand.Next()).ToList();

        Console.WriteLine("\n=== Turn order ===");
        for(int i=0;i<players.Count;i++)
        {
            Console.WriteLine($"{i+1}: {players[i].DisplayName(humanCount)}");
        }

        // 5. Turn-based เลือกพื้นที่
        List<string> availableAreas = areaNames.ToList();
        int turnIndex = 0;

        while(availableAreas.Count > 0)
        {
            var player = players[turnIndex];
            Console.WriteLine($"\n{player.DisplayName(humanCount)} เลือกพื้นที่ของตนเอง:");

            string choice = "";
            if(player.IsHuman)
            {
                Console.WriteLine("พื้นที่ว่าง: " + string.Join(",", availableAreas));
                Console.Write("เลือกพื้นที่: ");
                choice = Console.ReadLine().ToUpper();
                while(!availableAreas.Contains(choice))
                {
                    Console.WriteLine("เลือกไม่ได้ ลองอีกครั้ง");
                    Console.Write("เลือกพื้นที่: ");
                    choice = Console.ReadLine().ToUpper();
                }
            }
            else
            {
                // AI เลือกสุ่ม
                choice = availableAreas[rand.Next(availableAreas.Count)];
                Console.WriteLine($"{player.DisplayName(humanCount)} เลือก {choice}");
            }

            // อัปเดต
            availableAreas.Remove(choice);
            player.Areas.Add(choice);
            var areaObj = map_area.First(a => a.Name == choice);
            areaObj.OwnerTeam = players.IndexOf(player)+1;

            // ไปผู้เล่นถัดไป
            turnIndex = (turnIndex + 1) % players.Count;
        }

        // 6. แสดงผล grid
        Console.WriteLine("\n=== แผนที่ ===");
        for(int i=0;i<map_area.Length;i++)
        {
            Console.Write(map_area[i] + " ");
            if((i+1)%4==0) Console.WriteLine();
        }

        // 7. แสดง user list
        Console.WriteLine("\n=== User list ===");
        foreach(var player in players)
        {
            int totalTroops = player.Areas.Count; // ทหารเริ่มต้น 0 ยังไม่ลงจริง
            Console.WriteLine($"{player.DisplayName(humanCount)} : {string.Join(",", player.Areas)} จำนวนทหาร {totalTroops}");
        }
    }
}

