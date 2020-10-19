import {
	GET_USER_FAVOURITES,
	USER_ERROR,
	POST_ERROR,
	ADD_POST_TO_FAVOURITES,
	DELETE_POST_FROM_FAVOURITES
} from '../actions/types';

const initialState = {
	favourites: [],
	favouritesLoading: true,
	error: {}
};

export default function(state = initialState, action) {
	const { type, payload } = action;

	switch (type) {
		case GET_USER_FAVOURITES:
			return {
				...state,
				favourites: payload,
				favouritesLoading: false
			};
		case ADD_POST_TO_FAVOURITES:
			return {
				...state,
				favourites: [ payload, ...state.favourites ],
				favouritesLoading: false
			};
		case DELETE_POST_FROM_FAVOURITES:
			return {
				...state,
				favourites: state.favourites.filter((favourite) => favourite.id !== payload),
				favouritesLoading: false
			};
		case USER_ERROR:
		case POST_ERROR:
			return {
				...state,
				error: payload,
				favouritesLoading: false
			};
		default:
			return state;
	}
}
