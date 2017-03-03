using System;

public class Province
{
    /**
     * Rate at which this province generates each resource
     * Indexed by the ResourceType enum
     */
    private int[] resourceGenerationRates;

    /**
     * Resources held by this province
     */
    private ResourceBag resources;

    public Province()
    {
        City = null;
        Owner = null;
        this.resourceGenerationRates = new int[Enum.GetNames(typeof(ResourceType)).Length];
        this.resources = new ResourceBag();

        // for now, default regen rates = 10 for food and 5 for weapons if there is a city, otherwise 0
        // it may be changed in the future (need to discuss design) s.t. cities generate resources and they permeate upwards or tweak interface
        this.resourceGenerationRates[(int)ResourceType.Food] = 10;
    }

    public Province(City city, Player owner)
        : this()
    {
        City = city;
        Owner = owner;

        if (city != null)
        {
            this.resourceGenerationRates[(int)ResourceType.Weapons] = 5;
        }
    }

    /**
     * The city contained in this province
     * If the province doesn't have a city then null
     */
    public City City { get; set; }

    /**
     * The player that owns or occupies this province
     * If the province is neutral then null
     */
    public Player Owner { get; set; }

    /**
     * Tick method - updates state of Province after each round of turns completes
     * Right now, merely generates new resources.
     */
    public void Tick()
    {
        // loop over each resource type and regen as appropriate
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            this.resources.Add(resourceType, this.resourceGenerationRates[(int)resourceType]);
        }
    }

    /**
     * Called by an Army to gather all of the resources of a given type available at this province.
     * Returns amount gathered
     */
    public int Gather(ResourceType resourceType)
    {
        int ret = this.resources.GetAmountOf(resourceType);
        this.resources.Use(resourceType, ret);

        return ret;
    }
}
