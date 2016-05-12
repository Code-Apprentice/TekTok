package md59ed621e29e1702627842f2a0ab5a67d9;


public class ticketRequestActivity_VideoLoop
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.media.MediaPlayer.OnPreparedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onPrepared:(Landroid/media/MediaPlayer;)V:GetOnPrepared_Landroid_media_MediaPlayer_Handler:Android.Media.MediaPlayer/IOnPreparedListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("TekTokDroid.ticketRequestActivity+VideoLoop, TekTokDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ticketRequestActivity_VideoLoop.class, __md_methods);
	}


	public ticketRequestActivity_VideoLoop () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ticketRequestActivity_VideoLoop.class)
			mono.android.TypeManager.Activate ("TekTokDroid.ticketRequestActivity+VideoLoop, TekTokDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onPrepared (android.media.MediaPlayer p0)
	{
		n_onPrepared (p0);
	}

	private native void n_onPrepared (android.media.MediaPlayer p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
