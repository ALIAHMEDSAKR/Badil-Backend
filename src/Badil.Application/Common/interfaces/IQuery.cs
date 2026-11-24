using MediatR;

namespace Badil.Application.Common.interfaces;
public interface IQuery<out TResponse> : IRequest<TResponse>;

