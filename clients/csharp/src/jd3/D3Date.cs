namespace org.jd3
{
	using System;
	
	public class D3Date
	{
		private long D3Valeur;
		
		public D3Date(System.DateTime init)
		{
			D3Valeur = (long) D3Date.dateToInt(init);
		}
		
		public D3Date(D3Date init)
		{
			D3Valeur = init.dateInt();
		}
		
		public D3Date(long init)
		{
			D3Valeur = init;
		}
		
		public D3Date(int init)
		{
			D3Valeur = (long) init;
		}
		
		public virtual long dateInt()
		{
			return this.D3Valeur;
		}
		
		public virtual System.DateTime dateVal()
		{
			return D3Date.intToDate((int) D3Valeur);
		}
		
		/// <summary> Conversion d'une date en format interne Pick
		/// </summary>
		static public int dateToInt(System.DateTime datejava)
		{
			return (int) ((datejava.Ticks - 621355968000000000) / 10000 / D3Constants._D3_DiffTime) + D3Constants._D3_DiffDate;
		}
		
		/// <summary> Conversion d'une date format pick en java
		/// </summary>
		static public System.DateTime intToDate(int pickint)
		{
			long tmp = (pickint - D3Constants._D3_DiffDate);
			System.DateTime val = new System.DateTime(tmp * D3Constants._D3_DiffTime * 10000 + 621355968000000000);
			return val;
		}
		
		/// <summary> Conversion de secondes en Date Java
		/// </summary>
		static public System.DateTime intToTime(int pickint)
		{
			System.DateTime val = new System.DateTime(pickint * 1000 * 10000 + 621355968000000000);
			return val;
		}
		
		/// <summary> Conversion d'un Date Java en seconde Pick
		/// </summary>
		static public int timeToInt(System.DateTime datejava)
		{
			return (int) ((datejava.Ticks - 621355968000000000) / 10000 / 1000);
		}
		
		/// <summary> Conversion d'un time en heure
		/// *
		/// </summary>
		static public System.String intToMTS(int time)
		{
			int min, hour;
			hour = (int) (time / 3600);
			time -= (hour * 3600);
			min = (int) (time / 60);
			time -= (min * 60);
			return System.Convert.ToString(hour) + ":" + System.Convert.ToString(min) + ":" + System.Convert.ToString(time);
		}
		
		static public System.String intToMT(int time)
		{
			int min, hour;
			hour = (int) (time / 3600);
			time -= (hour * 3600);
			min = (int) (time / 60);
			time -= (min * 60);
			return System.Convert.ToString(hour) + ":" + System.Convert.ToString(min);
		}
		
		static public int MTSToInt(System.String heure)
		{
			int sec = 0;
			
			//SupportClass.Tokenizer sh = new SupportClass.Tokenizer(heure, ":");
			char seps = ':';
			String[] values = heure.Split(seps);
			
			try
			{
				//sec += (System.Int32.Parse(sh.NextToken()) * 3600);
				sec += (System.Int32.Parse(values[0]) * 3600);
			}
			catch
			{
			}
			try
			{
				//sec += (System.Int32.Parse(sh.NextToken()) * 60);
				sec += (System.Int32.Parse(values[1]) * 60);
			}
			catch
			{
			}
			try
			{
				//sec += System.Int32.Parse(sh.NextToken());
				sec += System.Int32.Parse(values[2]);
			}
			catch
			{
			}
			
			return sec;
		}
	}
}
