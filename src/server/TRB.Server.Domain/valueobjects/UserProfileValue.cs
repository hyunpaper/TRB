using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRB.Server.Domain.valueobjects
{
    public class UserProfileValue
    {
        public DateTime? BirthDate { get; }
        public string? Gender { get; }
        public string? Address { get; }
        public string? Nickname { get; }
        public string? ProfileImage { get; }

        public UserProfileValue(
            DateTime? birthDate,
            string? gender,
            string? address,
            string? nickname,
            string? profileImage)
        {
            BirthDate = birthDate;
            Gender = gender;
            Address = address;
            Nickname = nickname;
            ProfileImage = profileImage;
        }
        public override bool Equals(object? obj)
        {
            if (obj is not UserProfileValue other) return false;
            return Nullable.Equals(BirthDate, other.BirthDate)
                && Gender == other.Gender
                && Address == other.Address
                && Nickname == other.Nickname
                && ProfileImage == other.ProfileImage;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BirthDate, Gender, Address, Nickname, ProfileImage);
        }
    }
}
