﻿using Microsoft.Extensions.DependencyInjection;

namespace FastEndpoints;

public abstract class BaseEventHandler
{
#pragma warning disable CS8618
    internal static IServiceProvider ServiceProvider; //set from .UseFastEndpoints() upon initialization
#pragma warning restore CS8618
}

/// <summary>
/// use this base class to handle events published by the notification system
/// </summary>
/// <typeparam name="TEvent">the type of the event to handle</typeparam>
public abstract class FastEventHandler<TEvent> : BaseEventHandler, IEventHandler where TEvent : class, new()
{
    void IEventHandler.Subscribe()
        => Event<TEvent>.OnReceived += (e, c) => HandleAsync(e, c);

    /// <summary>
    /// this method will be called when an event of the specified type is published.
    /// </summary>
    /// <param name="eventModel">the event model/dto received</param>
    /// <param name="ct">an optional cancellation token</param>
    public abstract Task HandleAsync(TEvent eventModel, CancellationToken ct);

    /// <summary>
    /// publish the given model/dto to all the subscribers of the event notification
    /// </summary>
    /// <param name="eventModel">the notification event model/dto to publish</param>
    /// <param name="waitMode">specify whether to wait for none, any or all of the subscribers to complete their work</param>
    ///<param name="cancellation">an optional cancellation token</param>
    /// <returns>a Task that matches the wait mode specified.
    /// Mode.WaitForNone returns an already completed Task (fire and forget).
    /// Mode.WaitForAny returns a Task that will complete when any of the subscribers complete their work.
    /// Mode.WaitForAll return a Task that will complete only when all of the subscribers complete their work.</returns>
    protected Task PublishAsync<TEventModel>(TEventModel eventModel, Mode waitMode = Mode.WaitForAll, CancellationToken cancellation = default) where TEventModel : class
        => Event<TEventModel>.PublishAsync(eventModel, waitMode, cancellation);

    /// <summary>
    /// try to resolve an instance for the given type from the dependency injection container. will return null if unresolvable.
    /// </summary>
    /// <typeparam name="TService">the type of the service to resolve</typeparam>
    protected TService? TryResolve<TService>() => ServiceProvider.GetService<TService>();

    /// <summary>
    /// try to resolve an instance for the given type from the dependency injection container. will return null if unresolvable.
    /// </summary>
    /// <param name="typeOfService">the type of the service to resolve</param>
    protected object? TryResolve(Type typeOfService) => ServiceProvider.GetService(typeOfService);

    /// <summary>
    /// resolve an instance for the given type from the dependency injection container. will throw if unresolvable.
    /// </summary>
    /// <typeparam name="TService">the type of the service to resolve</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if requested service cannot be resolved</exception>
    protected TService Resolve<TService>() where TService : notnull => ServiceProvider.GetRequiredService<TService>();

    /// <summary>
    /// resolve an instance for the given type from the dependency injection container. will throw if unresolvable.
    /// </summary>
    /// <param name="typeOfService">the type of the service to resolve</param>
    /// <exception cref="InvalidOperationException">Thrown if requested service cannot be resolved</exception>
    protected object Resolve(Type typeOfService) => ServiceProvider.GetRequiredService(typeOfService);
}
