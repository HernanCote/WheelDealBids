namespace BuildingBlocks.Utils;

public class Result
{
    public Result() : this(false) { }

    public bool Succeeded { get; protected set; }
    public bool NotSucceeded => !Succeeded;
    
    public IEnumerable<Error> Errors => ErrorsList;
    
    protected Result(bool succeeded)
    {
        Succeeded = succeeded;
    }

    protected Result(bool succeeded, List<Error> errors) : this(succeeded)
    {
        ErrorsList = errors;
    }

    protected readonly List<Error> ErrorsList = new();

    private Result(Error error) : this(false)
    {
        ErrorsList.Add(error);
    }

    private Result(List<Error> errors) : this (false, errors) { }
    
    public Result Ok()
    {
        Succeeded = true;
        return this;
    }
    
    public Result Fail(string errorCode, string errorMsg)
    {
        return Fail(new Error(errorCode, errorMsg));
    }

    public Result Fail(params Error[] errors)
    {
        return Fail(errors.ToList());
    }
    
    public Result Fail(IList<Error>? errors)
    {
        Succeeded = false;

        if (errors is not null)
            ErrorsList.AddRange(errors);

        return this;
    }

    public static implicit operator Result(Error error) => new(error);
    public static implicit operator Result(List<Error> errors) => new(errors);
}

public class Result<T> : Result
{
	public Result() : this(false) { }

	protected Result(bool succeeded) : base(succeeded)
	{
		Value = default(T);
	}

	protected Result(bool succeeded, List<Error> errors) : base(succeeded, errors)
	{
		Value = default(T);
	}

	protected Result(T? value) : this(true)
	{
		Value = value;
	}

	private Result(Error error) : this(false, new List<Error> { error })
	{
		Value = default(T);
	}

	private Result(IEnumerable<Error>? errors) : this(false)
	{
		Value = default(T);
		
		if (errors is not null)
			ErrorsList.AddRange(errors);
	}

	public T? Value { get; set; }

	public Result<T> Ok(T value)
	{
		Succeeded = true;
		Value = value;
		return this;
	}

	public new Result<T> Fail(string errorMsg, string? errorCode = null)
	{
		if (errorCode is null)
			return Fail(Error.BadRequest(errorMsg));
		
		return Fail(new Error { Message = errorMsg, Code = errorCode });
	}

	public new Result<T> Fail(params Error[] errors)
	{
		return Fail(errors.ToList());
	}

	public Result<T> Fail(List<Error>? errors)
	{
		Succeeded = false;

		if (errors != null)
			ErrorsList.AddRange(errors);

		return this;
	}

	public Result<T> Fail(IEnumerable<Error>? errors)
	{
		Succeeded = false;

		if (errors is not null)
			ErrorsList.AddRange(errors);

		return this;
	}

	public Result<T> Fail(Result resError)
	{
		return Fail(resError.Errors);
	}

	public static Result<T?> Success(T? value)
	{
		return new Result<T?>(true) { Value = value };
	}

	public static Result<T> Success(T value, List<Error> errors)
	{
		return new Result<T>(true, errors) { Value = value };
	}

	public static Result<T> Failed(string errorMsg, string? errorCode = null)
	{
		return Failed(new Error { Message = errorMsg, Code = errorCode });
	}

	public static Result<T> Failed(params Error[] errors)
	{
		return Failed(errors.ToList());
	}

	public static Result<T> Failed(IEnumerable<Error>? errors)
	{
		Result<T> result = new Result<T>(false);

		if (errors != null)
			result.ErrorsList.AddRange(errors);

		return result;
	}

	public static Result<T?> From(Result fromResult)
	{
		if (fromResult.Succeeded)
			return Success(default);

		return new Result<T?>(false, fromResult.Errors.ToList());
	}

	public static implicit operator Result<T?>(T value) => new(value);
	public static implicit operator Result<T>(Error error) => new(error);
	public static implicit operator Result<T>(List<Error>? errors) => new(errors);
}