using Application.Core.Mapper;
using Application.Core.Validations;
using Domain.Exceptions;
using StringValidator = Application.Core.Validations.StringValidator;

namespace TestApi.ExcludeConstructorTest;

[TestFixture]
public class GeneralValidations
{
    private TopicDtoValidator? _topicDtoValidator;
    private StringValidator? _stringValidator;
    private ManagerMapper? _managerMapper;
    private EntityNullException? _entityNullException;

    [Test]
    public void SetUp()
    {
        _topicDtoValidator = new TopicDtoValidator();
        _stringValidator = new StringValidator();
        _managerMapper = new ManagerMapper();
        _entityNullException = new EntityNullException("EntityNullException");
    }
}