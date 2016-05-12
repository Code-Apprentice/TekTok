package md59ed621e29e1702627842f2a0ab5a67d9;


public class ticketRequestActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onBackPressed:()V:GetOnBackPressedHandler\n" +
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onPause:()V:GetOnPauseHandler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"n_onRestart:()V:GetOnRestartHandler\n" +
			"";
		mono.android.Runtime.register ("TekTokDroid.ticketRequestActivity, TekTokDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ticketRequestActivity.class, __md_methods);
	}


	public ticketRequestActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ticketRequestActivity.class)
			mono.android.TypeManager.Activate ("TekTokDroid.ticketRequestActivity, TekTokDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onBackPressed ()
	{
		n_onBackPressed ();
	}

	private native void n_onBackPressed ();


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onPause ()
	{
		n_onPause ();
	}

	private native void n_onPause ();


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();


	public void onRestart ()
	{
		n_onRestart ();
	}

	private native void n_onRestart ();

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
