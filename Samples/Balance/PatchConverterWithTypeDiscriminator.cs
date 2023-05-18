namespace Balance
{
    public class PatchConverterWithTypeDiscriminator : JsonConverter<AngouriMathPatch>
    {
        enum TypeDiscriminator
        {
            Customer = 1,
            Employee = 2
        }

        public override bool CanConvert(Type typeToConvert) =>
            typeof(AngouriMathPatch).IsAssignableFrom(typeToConvert);

        public override AngouriMathPatch Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string? propertyName = reader.GetString();
            if (propertyName != "TypeDiscriminator")
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            TypeDiscriminator typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
            AngouriMathPatch AngouriMathPatch = typeDiscriminator switch
            {
                TypeDiscriminator.Customer => new Customer(),
                TypeDiscriminator.Employee => new Employee(),
                _ => throw new JsonException()
            };

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return AngouriMathPatch;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "CreditLimit":
                            decimal creditLimit = reader.GetDecimal();
                            ((Customer)AngouriMathPatch).CreditLimit = creditLimit;
                            break;
                        case "OfficeNumber":
                            string? officeNumber = reader.GetString();
                            ((Employee)AngouriMathPatch).OfficeNumber = officeNumber;
                            break;
                        case "Name":
                            string? name = reader.GetString();
                            AngouriMathPatch.Name = name;
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer, AngouriMathPatch AngouriMathPatch, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (AngouriMathPatch is Customer customer)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Customer);
                writer.WriteNumber("CreditLimit", customer.CreditLimit);
            }
            else if (AngouriMathPatch is Employee employee)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Employee);
                writer.WriteString("OfficeNumber", employee.OfficeNumber);
            }

            writer.WriteString("Name", AngouriMathPatch.Name);

            writer.WriteEndObject();
        }
    }
}