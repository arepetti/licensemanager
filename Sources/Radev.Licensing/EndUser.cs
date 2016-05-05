//
// Copyright (c) 2015 Repetti Adriano.
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;

namespace Radev.Licensing
{
	/// <summary>
	/// Contains information about end-user to whom license has been issued.
	/// </summary>
	public sealed class EndUser
	{
		/// <summary>
		/// Gets/sets user's full name.
		/// </summary>
		/// <value>
		/// User's full name, in any form is valid for her culture or for company policy.
		/// It's not restricted in length and it may be <see langword="null"/> if this
		/// information is not available. This is the physical person to whom license
		/// has been issued, when it's not applicable then simply leave this field empty.
		/// <br/>
		/// Default value is <see langword="null"/>.
		/// </value>
		public string FullName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets/sets user's organization name.
		/// </summary>
		/// <value>
		/// User's organization name, in any form is valid for her culture or for company policy.
		/// It's not restricted in length and it may be <see langword="null"/> if this
		/// information is not available. This is the legal organization to which license
		/// has been issued, when it's not applicable (for example in case of private end-users)
		/// simply leave this field empty.
		/// <br/>
		/// Default value is <see langword="null"/>.
		/// </value>
		public string Organization
		{
			get;
			set;
		}

		/// <summary>
		/// Gets/sets user's address name.
		/// </summary>
		/// <value>
		/// User's address (home or office, use is unspecified), in any form is valid for her culture
		/// or for company policy. It's not restricted in length and it may be <see langword="null"/>
		/// if this information is not available.
		/// <br/>
		/// Default value is <see langword="null"/>.
		/// </value>
		public string Address
		{
			get;
			set;
		}

		/// <summary>
		/// Gets/sets user's phone number.
		/// </summary>
		/// <value>
		/// User's phone number (home or office, use is unspecified), in any form is valid for her culture
		/// or for company policy. It's not restricted in length and it may be <see langword="null"/>
		/// if this information is not available.
		/// <br/>
		/// Default value is <see langword="null"/>.
		/// </value>
		public string PhoneNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Gets/sets user's e-mail address.
		/// </summary>
		/// <value>
		/// User's e-mail address (home or office, use is unspecified), in any form is valid for her culture
		/// or for company policy. It's not restricted in length and it may be <see langword="null"/>
		/// if this information is not available.
		/// <br/>
		/// Default value is <see langword="null"/>.
		/// </value>
		public string EMailAddress
		{
			get;
			set;
		}

		/// <summary>
		/// Gets/sets any other relevant note about end-user or about this license.
		/// </summary>
		/// <value>
		/// Optional additional information about end-user or this license. Leave empty
		/// when not applicable.
		/// <br/>
		/// Default value is <see langword="null"/>.
		/// </value>
		public string Notes
		{
			get;
			set;
		}
	}
}
