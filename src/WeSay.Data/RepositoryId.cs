using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public interface RepositoryId :
		IComparable<RepositoryId>,
		IEquatable<RepositoryId>
	{
	}
}
