//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project_Content_Sharing.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment
    {
        public int CommentID { get; set; }
        public int ImageID { get; set; }
        public int SubCommentID { get; set; }
        public int VoteComment { get; set; }
        public string CommentText { get; set; }
        public string CommentUrl { get; set; }
        public int UserID { get; set; }
    
        public virtual ImgDB ImgDB { get; set; }
        public virtual UserTable UserTable { get; set; }
    }
}