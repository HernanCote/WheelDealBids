namespace BuildingBlocks;

public abstract class UpdateModelBase
{
    public void UpdateEntity<TEntity>(TEntity entity)
    {
        var updateModelProperties = GetType().GetProperties();

        foreach (var property in updateModelProperties)
        {
            var value = property.GetValue(this);
            if (value == null) 
                continue;
            
            var entityProperty = typeof(TEntity).GetProperty(property.Name);
            entityProperty?.SetValue(entity, Convert.ChangeType(value, entityProperty.PropertyType));
        }
    }
}