package se.svt.caspar
{
	public class Version
	{
		public static var GEN:String   		= "2";
		public static var MAJOR:String 		= "0";
		public static var MINOR:String 		= "4";
		public static var REVISION:String 	= "3803";
		public static var TAG:String		= "RC";
		
		//OUTPUT FORMAT: 2.0.0.2014 RC
		public static function getVersion():String
		{
			return Version.GEN + "." + Version.MAJOR + "." + Version.MINOR + "." + Version.REVISION + " " + Version.TAG;
		}
	}
}