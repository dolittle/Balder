﻿namespace Balder.Core.Runtime
{
	public interface IActor
	{
		void Initialize();
		void LoadContent();
		void Loaded();
		void BeforeUpdate();
		void Update();
		void AfterUpdate();
	}
}
