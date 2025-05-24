import { persistentAtom } from "@nanostores/persistent";

interface TourState {
  cardsLoadedCount: number;
  completed: boolean;
}

export const tourStore = persistentAtom<TourState>(
  "tour",
  { cardsLoadedCount: 0, completed: false }, // Default initial state
  {
    encode: JSON.stringify,
    decode: JSON.parse
  }
);

export const incrementCardsLoadedCount = () => {
  const state = tourStore.get();
  tourStore.set({ 
    cardsLoadedCount: state.cardsLoadedCount + 1, 
    completed: state.completed
  });
};

export const completeTour = () => {
  tourStore.set({ 
    cardsLoadedCount: 5,
    completed: true 
  });
};

export const resetTourState = () => {
    const state = tourStore.get();
  tourStore.set({ cardsLoadedCount: 0, completed: state.completed });
};

export const getCardsLoadedCount = (): number => {
  const state = tourStore.get();
  return state.cardsLoadedCount;
};

export const isTourCompleted = (): boolean => {
  const state = tourStore.get();
  return state.completed;
};
