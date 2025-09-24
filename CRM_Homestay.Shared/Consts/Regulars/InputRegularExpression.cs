using System;

namespace CRM_Homestay.Core.Consts.Regulars;

public class InputRegularExpression
{
    public const string Name =
        @"^\w[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵýỷỹ\s]*$";

    public const string Address = ".+";
    public const string Digit = @"\d";
    public const string Email = @"^[\w,.]+@[\w,.]+$";
    public const string NumberPhone = @"^0\d{9,10}$";
    public const string Password = @"^[\w@!#$%^&*?]+$";
    public const string Word = @"^\w+$";
    public const string AccountNumber = @"^[a-zA-Z0-9]*$";
    public const string NotSpecialCharacter = @"^[a-zA-Z0-9\sàáảãạăắằẳẵặâấầẩẫậèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵđÀÁẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴĐ]*$"; // không chứa ký tự đặc biệt
    public const string Numeric = @"^[0-9]+$";
    public const string EnglishAlphabet = @"^[a-zA-Z]+$";
}